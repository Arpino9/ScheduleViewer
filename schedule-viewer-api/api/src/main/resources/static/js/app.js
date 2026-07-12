'use strict';

// ===== 状態 =====
const state = {
  year:  new Date().getFullYear(),
  month: new Date().getMonth() + 1,   // 1-12
  selectedDate: null,                  // "YYYY-MM-DD"
  activeTab: 'schedule',
  cache: {}  // key: "YYYY-MM-DD#tab" → data
};

// ===== 初期化 =====
document.addEventListener('DOMContentLoaded', () => {
  renderCalendar();

  document.getElementById('btn-prev').addEventListener('click', () => {
    if (--state.month < 1) { state.month = 12; state.year--; }
    renderCalendar();
  });
  document.getElementById('btn-next').addEventListener('click', () => {
    if (++state.month > 12) { state.month = 1; state.year++; }
    renderCalendar();
  });

  document.querySelectorAll('.tab').forEach(btn => {
    btn.addEventListener('click', () => switchTab(btn.dataset.tab));
  });

  document.getElementById('modal-overlay').addEventListener('click', closeModal);
  document.getElementById('event-modal-overlay').addEventListener('click', e => {
    if (e.target === document.getElementById('event-modal-overlay')) closeEventModal();
  });
  document.getElementById('box-attach-overlay').addEventListener('click', e => {
    if (e.target === document.getElementById('box-attach-overlay')) hideBoxAttachModal();
  });
  document.getElementById('pane-schedule').addEventListener('click', e => {
    const btn = e.target.closest('.box-attach-btn');
    if (btn) openBoxAttachModal(btn.dataset.eventId, btn.dataset.eventTitle);
  });
  document.addEventListener('keydown', e => {
    if (e.key === 'Escape') { closeEventModal(); hideBoxAttachModal(); }
  });

  initSearch();
});

// ===== カレンダー描画 =====
function renderCalendar() {
  document.getElementById('cal-title').textContent =
    `${state.year}年 ${state.month}月`;

  const grid = document.getElementById('cal-grid');
  // 曜日ヘッダーを残して日付セルだけ削除
  [...grid.querySelectorAll('.day-cell')].forEach(el => el.remove());

  const firstDay = new Date(state.year, state.month - 1, 1).getDay(); // 0=Sun
  const daysInMonth = new Date(state.year, state.month, 0).getDate();
  const today = toDateStr(new Date());

  // 月曜始まり: 0=Mon … 6=Sun
  const offset = (firstDay === 0) ? 6 : firstDay - 1;

  // 前月の埋め
  const prevDays = new Date(state.year, state.month - 1, 0).getDate();
  for (let i = offset - 1; i >= 0; i--) {
    grid.appendChild(makeDayCell(
      state.year, state.month - 1 || 12,
      prevDays - i, true));
  }

  // 当月
  for (let d = 1; d <= daysInMonth; d++) {
    const cell = makeDayCell(state.year, state.month, d, false);
    const ds = toDateStr(new Date(state.year, state.month - 1, d));
    if (ds === today) cell.classList.add('today');
    if (ds === state.selectedDate) cell.classList.add('selected');
    grid.appendChild(cell);
  }

  // 翌月の埋め (6行 × 7列 = 42 マス)
  const total = offset + daysInMonth;
  const remaining = (Math.ceil(total / 7) * 7) - total;
  for (let d = 1; d <= remaining; d++) {
    grid.appendChild(makeDayCell(state.year, state.month + 1, d, true));
  }
}

function makeDayCell(y, m, d, otherMonth) {
  const dow = new Date(y, m - 1, d).getDay(); // 0=Sun
  const cell = document.createElement('div');
  cell.className = 'day-cell' +
    (otherMonth ? ' other-month' : '') +
    (dow === 6 ? ' sat' : '') +
    (dow === 0 ? ' sun' : '');

  const num = document.createElement('div');
  num.className = 'day-num';
  num.textContent = d;
  cell.appendChild(num);

  if (!otherMonth) {
    const normalY = (m > 12) ? y + 1 : (m < 1) ? y - 1 : y;
    const normalM = ((m - 1 + 12) % 12) + 1;
    const ds = `${normalY}-${String(normalM).padStart(2,'0')}-${String(d).padStart(2,'0')}`;
    cell.dataset.date = ds;
    cell.addEventListener('click', () => selectDate(ds, cell));
  }
  return cell;
}

// ===== 天気取得 (Open-Meteo / 愛知県一宮市) =====
const WEATHER_LAT  = 35.3033;
const WEATHER_LON  = 136.8008;
const WEATHER_TZ   = 'Asia%2FTokyo';

const WMO_CODE = {
  0: ['☀️','快晴'],
  1: ['🌤️','おおむね晴れ'],
  2: ['⛅','一部曇り'],
  3: ['☁️','曇り'],
  45: ['🌫️','霧'],
  48: ['🌫️','着氷霧'],
  51: ['🌦️','霧雨（弱）'],
  53: ['🌦️','霧雨'],
  55: ['🌦️','霧雨（強）'],
  56: ['🌦️','着氷霧雨'],
  57: ['🌦️','着氷霧雨（強）'],
  61: ['🌧️','小雨'],
  63: ['🌧️','雨'],
  65: ['🌧️','大雨'],
  66: ['🌧️','着氷雨'],
  67: ['🌧️','着氷雨（強）'],
  71: ['🌨️','小雪'],
  73: ['🌨️','雪'],
  75: ['🌨️','大雪'],
  77: ['🌨️','霧雪'],
  80: ['🌦️','にわか雨（弱）'],
  81: ['🌦️','にわか雨'],
  82: ['🌦️','にわか雨（強）'],
  85: ['🌨️','にわか雪'],
  86: ['🌨️','にわか大雪'],
  95: ['⛈️','雷雨'],
  96: ['⛈️','雷雨（ひょう）'],
  99: ['⛈️','激しい雷雨'],
};

async function fetchWeather(dateStr) {
  const el = document.getElementById('detail-weather');
  el.innerHTML = '<span class="wx-loading">天気取得中...</span>';

  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const target = new Date(dateStr + 'T00:00:00');

  let url;
  if (target <= today) {
    // 過去・当日 → アーカイブ API
    url = `https://archive-api.open-meteo.com/v1/archive` +
          `?latitude=${WEATHER_LAT}&longitude=${WEATHER_LON}` +
          `&start_date=${dateStr}&end_date=${dateStr}` +
          `&daily=weather_code,temperature_2m_max,temperature_2m_min` +
          `&timezone=${WEATHER_TZ}`;
  } else {
    // 未来 → 予報 API
    url = `https://api.open-meteo.com/v1/forecast` +
          `?latitude=${WEATHER_LAT}&longitude=${WEATHER_LON}` +
          `&daily=weather_code,temperature_2m_max,temperature_2m_min` +
          `&timezone=${WEATHER_TZ}` +
          `&start_date=${dateStr}&end_date=${dateStr}`;
  }

  try {
    const res = await fetch(url);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const data = await res.json();
    const daily = data.daily;
    if (!daily || !daily.weather_code || daily.weather_code.length === 0) {
      el.innerHTML = '';
      return;
    }
    const code = daily.weather_code[0];
    const tMax  = daily.temperature_2m_max[0];
    const tMin  = daily.temperature_2m_min[0];
    const [icon, label] = WMO_CODE[code] ?? ['🌡️', `コード${code}`];
    const tempHtml = (tMax != null && tMin != null)
      ? `<span class="wx-temp">${Math.round(tMax)}° / ${Math.round(tMin)}°</span>`
      : '';
    el.innerHTML =
      `<span class="wx-icon" title="${label}">${icon}</span>` +
      `<span class="wx-label">${label}</span>` +
      tempHtml;
  } catch (e) {
    console.warn('天気取得失敗:', e);
    el.innerHTML = '';
  }
}

// ===== サイドバートグル (モバイル用) =====
function toggleSidebar() {
  document.getElementById('sidebar').classList.toggle('open');
  document.getElementById('sidebar-overlay').classList.toggle('open');
}
function closeSidebar() {
  document.getElementById('sidebar').classList.remove('open');
  document.getElementById('sidebar-overlay').classList.remove('open');
}

// ===== 日付選択 =====
function selectDate(dateStr, cell) {
  document.querySelectorAll('.day-cell.selected').forEach(el => el.classList.remove('selected'));
  cell.classList.add('selected');

  state.selectedDate = dateStr;
  const d = new Date(dateStr);
  const dows = ['日','月','火','水','木','金','土'];
  document.getElementById('detail-date').textContent =
    `${d.getFullYear()}年${d.getMonth()+1}月${d.getDate()}日 (${dows[d.getDay()]})`;

  closeSidebar();
  fetchWeather(dateStr);
  loadTab(state.activeTab);
}

// ===== タブ切替 =====
function switchTab(tab) {
  state.activeTab = tab;
  document.querySelectorAll('.tab').forEach(b =>
    b.classList.toggle('active', b.dataset.tab === tab));
  document.querySelectorAll('.pane').forEach(p =>
    p.classList.toggle('active', p.id === `pane-${tab}`));

  if (state.selectedDate) loadTab(tab);
}

// ===== データ読み込みディスパッチ =====
function loadTab(tab) {
  const ds = state.selectedDate;
  if (!ds) return;
  const pane = document.getElementById(`pane-${tab}`);

  switch (tab) {
    case 'schedule':    loadSchedule(ds, pane);    break;
    case 'tasks':       loadTasks(ds, pane);       break;
    case 'health':      loadHealth(ds, pane);      break;
    case 'books':       loadBooks(ds, pane);       break;

    case 'anime':       loadAnime(ds, pane);       break;
  }
}

// ===== API ヘルパー =====
async function apiFetch(path) {
  try {
    const res = await fetch(path);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    return await res.json();
  } catch (e) {
    console.error(path, e);
    return null;
  }
}

function setLoading(pane) {
  pane.innerHTML = '<div class="loading">読み込み中...</div>';
}

function setEmpty(pane, msg = 'データがありません') {
  pane.innerHTML = `<div class="empty">${msg}</div>`;
}

// ===== スケジュールタブ =====
async function loadSchedule(ds, pane) {
  setLoading(pane);
  const [events, localPhotos, expenditureItems] = await Promise.all([
    apiFetch(`/api/calendar?date=${ds}`),
    apiFetch(`/api/photos/local/date/${ds}`),
    apiFetch(`/api/drive/expenditure/date/${ds}`)
  ]);
  if (!events) { setEmpty(pane, 'カレンダーデータの取得に失敗しました'); return; }

  const allDay = events.filter(e => (e.allDay || e.allDayEvent) && !e.program && !e.book);
  const timed  = events.filter(e => !e.allDay && !e.allDayEvent && !e.program && !e.book);

  let html = '';
  if (allDay.length) {
    html += '<div class="section-title">終日</div>';
    allDay.forEach(e => { html += renderEventCard(e); });
  }
  if (timed.length) {
    html += '<div class="section-title">時間指定</div>';
    timed.forEach(e => { html += renderEventCard(e); });
  }
  if (!allDay.length && !timed.length) {
    html += '<div class="empty">この日のスケジュールはありません</div>';
  }

  // 写真セクション
  const calPhotoUrls = extractPhotoUrls(events);
  const hasLocal = localPhotos && localPhotos.length > 0;
  const hasCal   = calPhotoUrls.length > 0;
  if (hasLocal || hasCal) {
    html += '<div class="section-title">写真</div>';
    if (hasLocal) {
      html += '<div class="photo-grid">' +
        localPhotos.map(p => `
          <div class="photo-card" onclick="openModal('${esc(p.url || '')}')">
            ${p.url
              ? `<img src="${esc(p.url)}" alt="${esc(p.fileName || '')}" loading="lazy">`
              : `<div class="photo-no-img">🖼️<span>${esc(p.fileName || 'No image')}</span></div>`}
            <div class="photo-name">${esc(p.fileName || '')}</div>
          </div>`).join('') +
        '</div>';
    }
    if (hasCal) {
      html += '<div class="gphoto-list">' +
        calPhotoUrls.map(item => `
          <a class="gphoto-link-card" href="${esc(item.url)}" target="_blank" rel="noopener noreferrer">
            <span class="gphoto-icon">📷</span>
            <span class="gphoto-label">${esc(item.title)}</span>
            <span class="gphoto-url">${esc(item.url)}</span>
            <span class="gphoto-arrow">↗</span>
          </a>`).join('') +
        '</div>';
    }
  }

  // 添付ファイルセクション
  const allAttachments = events.flatMap(e => (e.attachments || []).map(a => ({
    title:    a.title    || '',
    url:      a.url      || '',
    mimeType: a.mimeType || '',
    eventTitle: e.title  || ''
  })));
  if (allAttachments.length > 0) {
    html += '<div class="section-title">添付ファイル</div>';
    html += '<div class="gphoto-list">' +
      allAttachments.map(a => `
        <a class="gphoto-link-card" href="${esc(a.url)}" target="_blank" rel="noopener noreferrer">
          <span class="gphoto-icon">${attachmentIcon(a.mimeType)}</span>
          <span class="gphoto-label">${esc(a.title || a.eventTitle)}</span>
          <span class="gphoto-url">${esc(a.url)}</span>
          <span class="gphoto-arrow">↗</span>
        </a>`).join('') +
      '</div>';
  }

  // 収支セクション
  if (expenditureItems && expenditureItems.length > 0) {
    const total = expenditureItems.reduce((sum, i) => sum + (i.price || 0), 0);
    html += '<div class="section-title">収支</div>';
    html += `
      <table class="exp-table">
        <thead>
          <tr>
            <th>内容</th><th>大項目</th><th>中項目</th>
            <th>金融機関</th><th style="text-align:right">金額</th>
            <th>メモ</th>
          </tr>
        </thead>
        <tbody>
          ${expenditureItems.map(i => `
            <tr>
              <td>${esc(i.itemName || '')}</td>
              <td>${esc(i.categoryLarge || '')}</td>
              <td>${esc(i.categoryMiddle || '')}</td>
              <td>${esc(i.financialInstitutions || '')}</td>
              <td class="exp-amount">${(i.price || 0).toLocaleString()} 円</td>
              <td>${esc(i.memo || '')}</td>
            </tr>`).join('')}
        </tbody>
      </table>
      <div class="exp-total">合計: ${total.toLocaleString()} 円</div>`;
  }

  pane.innerHTML = html;
}

function renderEventCard(e) {
  const cls = e.book ? 'book' : e.program ? 'program' : e.allDay ? 'all-day' : '';
  const start = (!e.allDay && !e.allDayEvent && e.startDate) ? formatTime(e.startDate) : '';
  const end   = (!e.allDay && !e.allDayEvent && e.endDate)   ? formatTime(e.endDate)   : '';
  const time  = start ? `${start}${end ? ' ～ ' + end : ''}` : '終日';
  const attachBtn = e.eventId
    ? `<button class="box-attach-btn" data-event-id="${esc(e.eventId)}" data-event-title="${esc(e.title || e.displayTitle || '')}">📎 Box添付</button>`
    : '';
  return `
    <div class="event-card ${cls}">
      <div class="event-time">${esc(time)}</div>
      <div class="event-title">${esc(e.title || e.displayTitle || '')}</div>
      ${e.place ? `<div class="event-place">📍 ${esc(e.place)}</div>` : ''}
      ${e.description ? `<div class="event-desc">${descToSafeHtml(e.description)}</div>` : ''}
      ${attachBtn}
    </div>`;
}

// ===== タスクタブ =====
async function loadTasks(ds, pane) {
  setLoading(pane);
  const tasks = await apiFetch(`/api/tasks/date/${ds}`);
  if (!tasks) { setEmpty(pane, 'タスクデータの取得に失敗しました'); return; }
  if (tasks.length === 0) { setEmpty(pane, 'この日のタスクはありません'); return; }

  pane.innerHTML = tasks.map(t => `
    <div class="task-card">
      <div class="task-check">✓</div>
      <div class="task-body">
        <div class="task-list-name">${esc(t.taskListName || '')}</div>
        <div class="task-name">${esc(t.taskName || '')}</div>
        ${t.details ? `<div class="task-detail">${esc(t.details)}</div>` : ''}
      </div>
    </div>`).join('');
}

// ===== 健康タブ =====
async function loadHealth(ds, pane) {
  setLoading(pane);
  const [sleep, activity, heart, weight] = await Promise.all([
    apiFetch(`/api/fitbit/sleep?date=${ds}`),
    apiFetch(`/api/fitbit/activity?date=${ds}`),
    apiFetch(`/api/fitbit/heart?date=${ds}`),
    apiFetch(`/api/fitbit/weight?date=${ds}`)
  ]);

  const cards = [];

  // sleeping は Duration → Jackson が ISO-8601文字列 "PT6H38M50S" としてシリアライズ
  if (sleep && sleep.sleeping) {
    const totalSec = parseDuration(sleep.sleeping);
    const h = Math.floor(totalSec / 3600);
    const m = Math.floor((totalSec % 3600) / 60);
    cards.push({ icon:'😴', label:'睡眠時間', value:`${h}h ${m}m`, unit:'' });
    if (sleep.startTime) cards.push({ icon:'🌙', label:'就寝', value:formatTime(sleep.startTime), unit:'' });
    if (sleep.endTime)   cards.push({ icon:'☀️', label:'起床', value:formatTime(sleep.endTime),   unit:'' });
  }
  if (activity) {
    if (activity.steps)      cards.push({ icon:'🚶', label:'歩数',       value:Math.round(activity.steps).toLocaleString(), unit:'歩' });
    if (activity.caloriesOut)cards.push({ icon:'🔥', label:'消費カロリー', value:Math.round(activity.caloriesOut).toLocaleString(), unit:'kcal' });
    if (activity.distance)   cards.push({ icon:'📏', label:'距離',       value:(activity.distance/1000).toFixed(2), unit:'km' });
    if (activity.elevation)  cards.push({ icon:'🏢', label:'階数',       value:Math.round(activity.elevation), unit:'フロア' });
  }
  if (heart && heart.restingHeartRate) {
    cards.push({ icon:'❤️', label:'安静時心拍数', value:Math.round(heart.restingHeartRate), unit:'bpm' });
  }
  if (weight) {
    if (weight.weight) cards.push({ icon:'⚖️', label:'体重', value:weight.weight.toFixed(1), unit:'kg' });
    if (weight.bmi)    cards.push({ icon:'📊', label:'BMI',  value:weight.bmi.toFixed(1),    unit:'' });
  }

  if (cards.length === 0) { setEmpty(pane, 'この日の健康データはありません'); return; }

  pane.innerHTML = `<div class="health-grid">` +
    cards.map(c => `
      <div class="health-card">
        <div class="health-icon">${c.icon}</div>
        <div class="health-label">${c.label}</div>
        <div class="health-value">${c.value}</div>
        <div class="health-unit">${c.unit}</div>
      </div>`).join('') +
    `</div>`;
}

// ===== 本タブ =====

/**
 * 本のdescriptionを解析する (.NET版 Model_ScheduleDetails_Book と同じロジック)
 * 【タグ】\n値 形式 + "key : value" 形式 + 【本の概要】〜【本の評価】の複数行
 */
function parseBookDesc(desc) {
  if (!desc) return {};
  const lines = desc.split('\n');
  const result = {};

  // 【タグ】\n値 形式
  for (let i = 0; i < lines.length; i++) {
    const m = lines[i].match(/^【([^】]+)】\s*$/);
    if (m) result[m[1].trim()] = (lines[i + 1] || '').trim();
  }

  // "key : value" 形式 (発売日・ISBN・本の種類)
  const bookTypeRe = /コミック|文庫|単行本|新書|大型本|電子書籍|ペーパーバック/;
  for (const line of lines) {
    const sep = line.indexOf(' : ');
    if (sep < 0) continue;
    const key = line.slice(0, sep).trim();
    const val = line.slice(sep + 3).trim();
    if (key === '発売日') result['発売日'] = val;
    else if (key === 'ISBN-10') result['ISBN-10'] = val;
    else if (key === 'ISBN-13') result['ISBN-13'] = val;
    else if (bookTypeRe.test(key)) result['本の種類'] = key;
  }

  // 【本の概要】〜【本の評価】間の複数行テキスト
  const ci = lines.findIndex(l => l.startsWith('【本の概要】'));
  const ei = lines.findIndex(l => l.startsWith('【本の評価】'));
  if (ci >= 0 && ei > ci) result['本の概要'] = lines.slice(ci + 1, ei).join('\n').trim();

  return result;
}

let bookCurrentEvents = [];
let bookCurrentDate   = '';

async function loadBooks(ds, pane) {
  setLoading(pane);
  const events = await apiFetch(`/api/calendar?date=${ds}`);
  if (!events) { setEmpty(pane, 'データ取得に失敗しました'); return; }

  const bookEvents = events.filter(e => e.book);
  if (bookEvents.length === 0) { setEmpty(pane, 'この日の読書記録はありません'); return; }

  bookCurrentEvents = bookEvents;
  bookCurrentDate   = ds;

  pane.innerHTML = `
    <div class="anime-layout">
      <div class="anime-table-wrap">
        <table class="anime-table">
          <thead>
            <tr><th>タイトル</th><th>著者</th><th>出版社</th></tr>
          </thead>
          <tbody>
            ${bookEvents.map((e, i) => {
              const d = parseBookDesc(e.description);
              return `<tr class="anime-row" data-idx="${i}" onclick="selectBookRow(this, ${i})">
                <td>${esc(e.title || '')}</td>
                <td>${esc(d['著者'] || '')}</td>
                <td>${esc(d['出版社'] || '')}</td>
              </tr>`;
            }).join('')}
          </tbody>
        </table>
      </div>
      <div class="anime-detail" id="book-detail">
        <div class="anime-detail-empty">行を選択すると詳細が表示されます</div>
      </div>
    </div>`;
}

async function selectBookRow(row, idx) {
  document.querySelectorAll('.anime-row').forEach(r => r.classList.remove('selected'));
  row.classList.add('selected');

  const event  = bookCurrentEvents[idx];
  const detail = document.getElementById('book-detail');
  const desc   = parseBookDesc(event.description);
  const title  = event.title || '';

  renderBookDetail(detail, null, title, desc);

  const book = await apiFetch(`/api/books?title=${encodeURIComponent(title)}&readDate=${bookCurrentDate}`);
  renderBookDetail(detail, book, title, desc);
}

function renderBookDetail(detail, book, title, desc) {
  const thumbUrl = desc['サムネイル'] || (book && book.thumbnail) || '';
  const thumb = thumbUrl
    ? `<img class="book-detail-thumb" src="${esc(thumbUrl)}" alt="${esc(title)}">`
    : `<div class="book-detail-thumb-placeholder">📖</div>`;

  const caption = desc['本の概要'] || (book && book.caption) || '';

  const rows = [
    ['タイトル',  esc(title)],
    ['日付',      esc(bookCurrentDate.replace(/-/g, '/'))],
    ['著者/作者', esc(desc['著者']    || (book && book.author)       || '')],
    ['出版社',    esc(desc['出版社']  || (book && book.publisher)    || '')],
    ['発売日',    esc(desc['発売日']  || (book && book.releasedDate) || '')],
    ['本の種類',  esc(desc['本の種類']|| (book && book.type)         || '')],
    ['ISBN-10',   esc(desc['ISBN-10'] || (book && book.isbn10)       || '')],
    ['ISBN-13',   esc(desc['ISBN-13'] || (book && book.isbn13)       || '')],
    ['概要',      esc(caption).replace(/\n/g, '<br>')],
    ['評価',      esc(desc['本の評価']|| (book && book.rating)       || '')],
  ];

  detail.innerHTML = `
    <div class="anime-detail-inner">
      <div class="anime-detail-left">${thumb}</div>
      <div class="anime-detail-right">
        <table class="anime-detail-table">
          ${rows.map(([label, val]) => `
            <tr><th>${label}</th><td>${val}</td></tr>`).join('')}
        </table>
      </div>
    </div>`;
}

/**
 * カレンダーイベントの説明欄から【写真】URLを抽出する
 * 対応フォーマット:
 *   【写真】\nhttps://...           (URLが次の行)
 *   【写真】https://...             (URLが同じ行)
 *   【写真】<br><a href="url">...</a>  (<br>区切り + <a>タグ)
 * @returns {{url: string, title: string}[]}
 */
function extractPhotoUrls(events) {
  const results = [];
  const seen = new Set();
  for (const e of events) {
    if (!e.description) continue;
    // <br> を \n に正規化してから行分割
    const normalized = e.description.replace(/<br\s*\/?>/gi, '\n');
    const lines = normalized.split('\n');
    for (let i = 0; i < lines.length; i++) {
      const m = lines[i].match(/^【写真】(.*)/);
      if (!m) continue;
      let inline = m[1].trim();
      // <a href="url"> からURL抽出
      const aMatch = inline.match(/href="([^"]+)"/);
      if (aMatch) {
        if (!seen.has(aMatch[1])) { seen.add(aMatch[1]); results.push({ url: aMatch[1], title: e.title || '' }); }
        continue;
      }
      if (inline.startsWith('http')) {
        if (!seen.has(inline)) { seen.add(inline); results.push({ url: inline, title: e.title || '' }); }
        continue;
      }
      // 次の行以降にURLが続く限り全て取得
      let j = i + 1;
      while (j < lines.length) {
        const nextLine = lines[j].trim();
        const nextAMatch = nextLine.match(/href="([^"]+)"/);
        if (nextAMatch) {
          if (!seen.has(nextAMatch[1])) { seen.add(nextAMatch[1]); results.push({ url: nextAMatch[1], title: e.title || '' }); }
          j++;
        } else if (nextLine.startsWith('http')) {
          if (!seen.has(nextLine)) { seen.add(nextLine); results.push({ url: nextLine, title: e.title || '' }); }
          j++;
        } else {
          break;
        }
      }
    }
  }
  return results;
}

// ===== アニメタブ =====

/**
 * description から【tag】値を抽出する
 * フォーマット: 【タグ】\n値 (改行区切り、.NET版と同じ)
 * 同行に値がある場合 (【タグ】値) にも対応
 */
function parseAnimeDesc(desc) {
  if (!desc) return {};
  const result = {};
  const lines = desc.split('\n');
  let currentKey = null;
  let currentLines = [];

  const flush = () => {
    if (currentKey) result[currentKey] = currentLines.join('\n').trim();
  };

  for (const line of lines) {
    const m = line.match(/^【([^】]+)】(.*)/);
    if (m) {
      flush();
      currentKey = m[1].trim();
      const inlineVal = m[2].trim();
      currentLines = inlineVal ? [inlineVal] : [];
    } else if (currentKey) {
      currentLines.push(line);
    }
  }
  flush();
  return result;
}

/**
 * カレンダータイトルから話数 (数字のみ) を抽出する
 * "ワンピース 第1100話" → "1100"
 * "ワンピース シリーズ 第1100話" → "1100"
 */
function getPart(title) {
  const parts = title.split(' ');
  const raw = parts.length > 2 ? parts[2] : (parts.length > 1 ? parts[1] : '');
  return raw.replace(/[^0-9]/g, '');
}

/**
 * Annict 完全一致用タイトルを取得する (.NET版 GetTitle と同じ)
 * "ワンピース 第1100話" → "ワンピース"
 * "ワンピース シリーズ 第1100話" → "ワンピース シリーズ"
 */
function getAnimeMatchTitle(title) {
  const parts = title.split(' ');
  return parts.length > 2 ? parts[0] + ' ' + parts[1] : parts[0];
}

let animeCurrentEvents = [];

async function loadAnime(ds, pane) {
  setLoading(pane);
  const events = await apiFetch(`/api/calendar/anime?date=${ds}`);
  if (!events) { setEmpty(pane, 'データ取得に失敗しました'); return; }
  if (events.length === 0) { setEmpty(pane, 'この日の視聴記録はありません'); return; }

  animeCurrentEvents = events;

  pane.innerHTML = `
    <div class="anime-layout">
      <div class="anime-table-wrap">
        <table class="anime-table">
          <thead>
            <tr>
              <th>タイトル</th>
              <th>話数</th>
              <th>サブタイトル</th>
              <th>視聴先</th>
            </tr>
          </thead>
          <tbody id="anime-tbody">
            ${events.map((e, i) => {
              const d = parseAnimeDesc(e.description);
              const part = getPart(e.title || '');
              return `<tr class="anime-row" data-idx="${i}" onclick="selectAnimeRow(this, ${i})">
                <td>${esc(e.title || '')}</td>
                <td>${esc(part)}</td>
                <td>${esc(d['サブタイトル'] || '')}</td>
                <td>${esc(d['視聴先'] || '')}</td>
              </tr>`;
            }).join('')}
          </tbody>
        </table>
      </div>
      <div class="anime-detail" id="anime-detail">
        <div class="anime-detail-empty">行を選択すると詳細が表示されます</div>
      </div>
    </div>`;
}

async function selectAnimeRow(row, idx) {
  // ハイライト
  document.querySelectorAll('.anime-row').forEach(r => r.classList.remove('selected'));
  row.classList.add('selected');

  const event = animeCurrentEvents[idx];
  const detail = document.getElementById('anime-detail');
  const desc = parseAnimeDesc(event.description);
  const calTitle = event.title || '';
  const part = getPart(calTitle);
  const normalizedTitle = calTitle.replace(/_/g, ' ').replace(/　/g, ' ');  // _・全角スペース→半角スペースで正規化
  const matchTitle  = getAnimeMatchTitle(normalizedTitle);        // Annict 完全一致タイトル
  const searchWord  = matchTitle;                                  // Annict 検索キー (シリーズタイトル)

  // まずカレンダー情報だけ表示
  renderAnimeDetail(detail, null, calTitle, desc, null, '', part);

  // Annict + スプレッドシート (サムネイル・各話サムネイル) を並列取得
  const [animes, thumbData, episodeThumbData] = await Promise.all([
    apiFetch(`/api/anime?title=${encodeURIComponent(searchWord)}&first=5&castFirst=10`),
    apiFetch(`/api/spreadsheet/thumbnail?title=${encodeURIComponent(matchTitle)}`),
    apiFetch(`/api/spreadsheet/episode-thumbnail?title=${encodeURIComponent(calTitle)}`)
  ]);

  // Annictの結果からタイトルを探す (全角スペース正規化後に比較)
  // 1. 完全一致
  // 2. matchTitle が Annict タイトルで始まる (例: matchTitle "VRAINS Ai編" → Annict "VRAINS")
  // 3. Annict タイトルが matchTitle で始まる (例: matchTitle "キングダム" → Annict "キングダム 第5シリーズ")
  //    → シリーズが複数ある場合は最新（末尾）を優先
  const normStr = s => s.replace(/　/g, ' ');
  const a = animes ? (
    animes.find(x => normStr(x.title) === matchTitle) ||
    animes.find(x => matchTitle.startsWith(normStr(x.title))) ||
    animes.findLast(x => normStr(x.title).startsWith(matchTitle + ' ')) ||
    null
  ) : null;
  const thumbnail = (thumbData && thumbData.url) ? thumbData.url
                  : (a && a.thumbnail)           ? a.thumbnail
                  : null;
  const caption = desc['概要'] || '';
  const episodeThumbnail = (episodeThumbData && episodeThumbData.url) ? episodeThumbData.url : null;
  renderAnimeDetail(detail, a, a ? a.title : calTitle, desc, thumbnail, caption, part, episodeThumbnail);
}

function renderAnimeDetail(detail, a, title, desc, thumbnail, caption = '', part = '', episodeThumbnail = null) {
  const thumb = thumbnail
    ? `<img class="anime-detail-thumb" src="${esc(thumbnail)}" alt="${esc(title)}">`
    : `<div class="anime-detail-thumb-placeholder">📺</div>`;
  const episodeThumb = episodeThumbnail
    ? `<img class="anime-episode-thumb" src="${esc(episodeThumbnail)}" alt="${esc(title)}">`
    : '';

  const rows = [
    ['タイトル',   esc(title)],
    ['話数',       esc(a && a.episodesCount ? part + ' / ' + a.episodesCount : part)],
    ['サブタイトル', esc(desc['サブタイトル'] || '')],
    ['キャスト',   a && a.cast ? esc(a.cast) : ''],
    ['視聴先',     esc(desc['視聴先'] || '')],
    ['制作年',     a && (a.seasonYear || a.seasonName) ? esc((a.seasonYear || '') + ' ' + (a.seasonName || '')).trim() : ''],
    ['公式サイト', a && a.officialSiteUrl ? `<a href="${esc(a.officialSiteUrl)}" target="_blank" rel="noopener">${esc(a.officialSiteUrl)}</a>` : ''],
    ['Wikipedia',  a && a.wikipediaUrl    ? `<a href="${esc(a.wikipediaUrl)}"    target="_blank" rel="noopener">${esc(a.wikipediaUrl)}</a>`    : ''],
    ['概要',       esc(caption)],
  ];

  detail.innerHTML = `
    <div class="anime-detail-inner">
      <div class="anime-detail-left">${thumb}${episodeThumb}</div>
      <div class="anime-detail-right">
        <table class="anime-detail-table">
          ${rows.map(([label, val]) => `
            <tr>
              <th>${label}</th>
              <td>${val}</td>
            </tr>`).join('')}
        </table>
      </div>
    </div>`;
}

// ===== 認証パネル =====
const AUTH_LABELS = {
  calendar: '📅 Google Calendar',
  tasks:    '✅ Google Tasks',
  drive:    '💾 Google Drive',
  photos:   '🖼️ Google Photos',
  sheets:   '📊 Google Sheets',
  fitbit:   '💪 Fitbit'
};

async function showAuthPanel() {
  document.getElementById('auth-overlay').classList.add('open');
  await refreshAuthStatus();
}
function hideAuthPanel() {
  document.getElementById('auth-overlay').classList.remove('open');
}

// 認証URL表示 (サービスごとにキャッシュ)
const authUrlCache = {};

function renderAuthList(status) {
  const list = document.getElementById('auth-list');
  list.innerHTML = Object.entries(status).map(([svc, ok]) => {
    const cachedUrl = authUrlCache[svc];
    const linkHtml = cachedUrl
      ? `<a class="auth-link" href="${cachedUrl}" target="_blank" rel="noopener">🔗 認証ページを開く</a>`
      : '';
    return `
    <div class="auth-row">
      <span class="auth-service-name">${AUTH_LABELS[svc] || svc}</span>
      <div style="display:flex;align-items:center;gap:6px;flex-wrap:wrap">
        <span class="auth-badge ${ok ? 'ok' : 'ng'}">${ok ? '認証済み' : '未認証'}</span>
        ${!ok && !cachedUrl ? `<button class="auth-row-btn" onclick="authorizeService('${svc}', this)">認証する</button>` : ''}
        ${linkHtml}
      </div>
    </div>`;
  }).join('');
}

async function refreshAuthStatus() {
  const status = await apiFetch('/api/auth/status');
  if (!status) return;
  renderAuthList(status);
}

async function requestAuthorization(path) {
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), 20000);

  try {
    const res = await fetch(path, {
      method: 'POST',
      signal: controller.signal
    });
    if (!res.ok) throw new Error(`サーバーエラー (HTTP ${res.status})`);
    return await res.json();
  } catch (err) {
    if (err.name === 'AbortError') {
      throw new Error('認証URLの取得がタイムアウトしました');
    }
    throw err;
  } finally {
    clearTimeout(timeoutId);
  }
}

async function authorizeService(service, btn) {
  btn.disabled = true;
  btn.textContent = '取得中...';

  try {
    const result = await requestAuthorization(`/api/auth/google/${service}`);
    if (result.url) {
      authUrlCache[service] = result.url;
    } else if (result.status === 'already_authorized') {
      delete authUrlCache[service];
    }
    await refreshAuthStatus();
  } catch (err) {
    console.error(`認証URL取得失敗: ${service}`, err);
    alert(`認証URLの取得に失敗しました: ${err.message}`);
  } finally {
    btn.disabled = false;
    btn.textContent = '認証する';
  }
}

async function authorizeAll(allBtn) {
  allBtn.disabled = true;
  allBtn.textContent = '取得中...';

  try {
    const result = await requestAuthorization('/api/auth/google/all');
    Object.entries(result).forEach(([svc, r]) => {
      if (r && r.url) authUrlCache[svc] = r.url;
      else delete authUrlCache[svc];
    });
    await refreshAuthStatus();
  } catch (err) {
    console.error('一括認証URL取得失敗', err);
    alert(`一括認証URLの取得に失敗しました: ${err.message}`);
  } finally {
    allBtn.disabled = false;
    allBtn.textContent = '一括認証';
  }
}

// ===== 全再読み込み =====
async function reloadAll() {
  state.cache = {};
  await Promise.allSettled([
    fetch('/api/calendar/reload', { method: 'POST' }),
    fetch('/api/tasks/reload',    { method: 'POST' }),
    fetch('/api/drive/expenditure/reload', { method: 'POST' }),
    fetch('/api/photos/reload',   { method: 'POST' }),
    fetch('/api/spreadsheet/thumbnail/reload',         { method: 'POST' }),
    fetch('/api/spreadsheet/caption/reload',           { method: 'POST' }),
    fetch('/api/spreadsheet/episode-thumbnail/reload', { method: 'POST' })
  ]);
  if (state.selectedDate) loadTab(state.activeTab);
  alert('再読み込みをリクエストしました。バックグラウンドで処理中です。');
}

// ===== 写真モーダル =====
function openModal(url) {
  if (!url) return;
  const overlay = document.getElementById('modal-overlay');
  document.getElementById('modal-img').src = url;
  overlay.classList.add('open');
}
function closeModal() {
  document.getElementById('modal-overlay').classList.remove('open');
}

// ===== ユーティリティ =====

function attachmentIcon(mimeType) {
  if (!mimeType) return '📎';
  if (mimeType.includes('pdf'))   return '📄';
  if (mimeType.includes('image')) return '🖼️';
  if (mimeType.includes('video')) return '🎬';
  if (mimeType.includes('audio')) return '🎵';
  if (mimeType.includes('spreadsheet') || mimeType.includes('excel')) return '📊';
  if (mimeType.includes('presentation') || mimeType.includes('powerpoint')) return '📽️';
  if (mimeType.includes('document') || mimeType.includes('word')) return '📝';
  return '📎';
}
function toDateStr(d) {
  return `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`;
}

// ISO-8601 Duration文字列 "PT6H38M50S" を秒数に変換
function parseDuration(d) {
  if (!d) return 0;
  if (typeof d === 'object') return d.seconds || 0;
  const m = d.match(/PT(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:\.\d+)?)S)?/);
  if (!m) return 0;
  return (parseInt(m[1] || 0) * 3600) + (parseInt(m[2] || 0) * 60) + parseFloat(m[3] || 0);
}

function formatTime(isoStr) {
  if (!isoStr) return '';
  const d = new Date(isoStr);
  if (isNaN(d)) return '';
  return `${String(d.getHours()).padStart(2,'0')}:${String(d.getMinutes()).padStart(2,'0')}`;
}

function esc(str) {
  return String(str)
    .replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;')
    .replace(/"/g,'&quot;').replace(/'/g,'&#39;');
}

// ===== アニメ視聴登録 =====

function showAnimeRegModal() {
  document.getElementById('reg-date').value = toDateStr(new Date());
  document.getElementById('anime-reg-overlay').classList.add('open');
  document.getElementById('reg-title').focus();
}

function hideAnimeRegModal() {
  document.getElementById('anime-reg-overlay').classList.remove('open');
  document.getElementById('anime-reg-form').reset();
  document.getElementById('reg-service').value = 'dアニメストア';
  const btn = document.getElementById('reg-submit-btn');
  btn.disabled = false;
  btn.textContent = '登録する';
}

async function submitAnimeReg(e) {
  e.preventDefault();
  const btn = document.getElementById('reg-submit-btn');
  btn.disabled = true;
  btn.textContent = '登録中...';

  const body = {
    date:     document.getElementById('reg-date').value,
    title:    document.getElementById('reg-title').value.trim(),
    episode:  parseInt(document.getElementById('reg-episode').value, 10),
    subtitle: document.getElementById('reg-subtitle').value.trim(),
    service:  document.getElementById('reg-service').value.trim(),
    summary:  document.getElementById('reg-summary').value.trim()
  };

  try {
    const res  = await fetch('/api/anime/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body)
    });
    const data = res.ok ? await res.json() : null;

    if (data && data.status === 'ok') {
      alert(data.message);
      hideAnimeRegModal();
      // 登録日が現在選択中の日付なら再読み込み
      if (state.selectedDate === body.date) loadTab('schedule');
    } else {
      alert('登録に失敗しました: ' + (data ? data.message : 'サーバーエラー'));
      btn.disabled = false;
      btn.textContent = '登録する';
    }
  } catch (err) {
    alert('通信エラーが発生しました: ' + err.message);
    btn.disabled = false;
    btn.textContent = '登録する';
  }
}

// ===== 検索 =====

let _searchTimer = null;
let _searchResults = [];
let _searchIdx = -1;

function initSearch() {
  const input    = document.getElementById('search-input');
  const dropdown = document.getElementById('search-dropdown');

  input.addEventListener('input', () => {
    clearTimeout(_searchTimer);
    const q = input.value.trim();
    if (q.length < 2) { closeSearchDropdown(); return; }
    _searchTimer = setTimeout(() => performSearch(q), 300);
  });

  input.addEventListener('keydown', e => {
    if (!dropdown.classList.contains('open')) return;
    if (e.key === 'ArrowDown') {
      e.preventDefault();
      _searchIdx = Math.min(_searchIdx + 1, _searchResults.length - 1);
      highlightSearchItem();
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      _searchIdx = Math.max(_searchIdx - 1, 0);
      highlightSearchItem();
    } else if (e.key === 'Enter') {
      e.preventDefault();
      if (_searchIdx >= 0 && _searchResults[_searchIdx]) {
        openEventModal(_searchResults[_searchIdx]);
      }
    } else if (e.key === 'Escape') {
      closeSearchDropdown();
      input.blur();
    }
  });

  document.addEventListener('click', e => {
    if (!e.target.closest('#search-wrap')) closeSearchDropdown();
  });
}

async function performSearch(q) {
  const data = await apiFetch(`/api/calendar/search?q=${encodeURIComponent(q)}`);
  _searchResults = data || [];
  _searchIdx = -1;
  renderSearchDropdown();
}

function renderSearchDropdown() {
  const dropdown = document.getElementById('search-dropdown');
  if (_searchResults.length === 0) {
    dropdown.innerHTML = '<div class="search-no-result">該当する予定はありません</div>';
  } else {
    dropdown.innerHTML = _searchResults.map((e, i) => {
      const isAllDay = e.allDay || e.allDayEvent;
      let meta = '';
      if (!isAllDay && e.startDate) {
        const d   = new Date(e.startDate);
        const ymd = `${d.getFullYear()}/${d.getMonth()+1}/${d.getDate()}`;
        const hm  = formatTime(e.startDate);
        const parts = [ymd, hm];
        if (e.place) parts.push(e.place);
        meta = parts.filter(Boolean).join(' / ');
      }
      return `<div class="search-item" data-idx="${i}" onclick="onSearchItemClick(${i})">
        <div class="search-item-title">${esc(e.title || e.displayTitle || '')}</div>
        ${meta ? `<div class="search-item-meta">${esc(meta)}</div>` : ''}
      </div>`;
    }).join('');
  }
  dropdown.classList.add('open');
}

function highlightSearchItem() {
  document.querySelectorAll('.search-item').forEach((el, i) => {
    el.classList.toggle('active', i === _searchIdx);
  });
}

function closeSearchDropdown() {
  document.getElementById('search-dropdown').classList.remove('open');
  _searchResults = [];
  _searchIdx = -1;
}

function onSearchItemClick(idx) {
  const event = _searchResults[idx];
  if (!event) return;
  navigateToEvent(event);
  closeSearchDropdown();
  document.getElementById('search-input').value = '';
}

function navigateToEvent(event) {
  const dateStr = event.startDate ? event.startDate.slice(0, 10) : null;
  if (!dateStr) return;
  const [y, m] = dateStr.split('-').map(Number);
  state.year  = y;
  state.month = m;
  renderCalendar();
  const cell = document.querySelector(`.day-cell[data-date="${dateStr}"]`);
  if (cell) selectDate(dateStr, cell);
}

// ===== イベント詳細モーダル =====

function openEventModal(event) {
  navigateToEvent(event);

  const isAllDay = event.allDay || event.allDayEvent;
  const dateStr  = event.startDate ? event.startDate.slice(0, 10) : '';
  const dows = ['日','月','火','水','木','金','土'];
  let dateLabel = '';
  if (dateStr) {
    const d = new Date(dateStr + 'T00:00:00');
    dateLabel = `${d.getFullYear()}年${d.getMonth()+1}月${d.getDate()}日 (${dows[d.getDay()]})`;
  }
  const timeLabel = isAllDay ? '終日'
    : formatTime(event.startDate) + (event.endDate ? ' 〜 ' + formatTime(event.endDate) : '');

  const rows = [
    ['日付', esc(dateLabel)],
    ['時間', esc(timeLabel)],
  ];
  if (event.place)       rows.push(['場所', esc(event.place)]);
  if (event.description) rows.push(['詳細', descToSafeHtml(event.description)]);

  document.getElementById('event-modal-title').textContent = event.title || event.displayTitle || '';
  document.getElementById('event-modal-body').innerHTML = `
    <table class="event-modal-table">
      ${rows.map(([k, v]) => `<tr><th>${k}</th><td>${v}</td></tr>`).join('')}
    </table>`;
  document.getElementById('event-modal-overlay').classList.add('open');

  closeSearchDropdown();
  document.getElementById('search-input').value = '';
}

function closeEventModal() {
  document.getElementById('event-modal-overlay').classList.remove('open');
}

// ===== Box添付モーダル =====

let _boxAttachEventId = null;

function openBoxAttachModal(eventId, eventTitle) {
  _boxAttachEventId = eventId;
  document.getElementById('box-attach-event-title').textContent = eventTitle;
  document.getElementById('box-attach-url').value = '';
  document.getElementById('box-attach-title').value = '';
  document.getElementById('box-attach-overlay').classList.add('open');
}

function hideBoxAttachModal() {
  document.getElementById('box-attach-overlay').classList.remove('open');
  _boxAttachEventId = null;
}

async function submitBoxAttach(e) {
  e.preventDefault();
  if (!_boxAttachEventId) return;
  const btn = document.getElementById('box-attach-submit-btn');
  btn.disabled = true;
  btn.textContent = '添付中...';
  try {
    const res = await fetch(`/api/calendar/events/${encodeURIComponent(_boxAttachEventId)}/attachments`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        fileUrl:   document.getElementById('box-attach-url').value,
        fileTitle: document.getElementById('box-attach-title').value
      })
    });
    if (!res.ok) throw new Error('HTTP ' + res.status);
    hideBoxAttachModal();
    if (state.selectedDate) {
      const pane = document.getElementById('pane-schedule');
      delete state.cache[state.selectedDate + '#schedule'];
      loadSchedule(state.selectedDate, pane);
    }
  } catch (err) {
    alert('添付に失敗しました: ' + err.message);
  } finally {
    btn.disabled = false;
    btn.textContent = '添付する';
  }
}

/**
 * Google Calendar の description (HTML混在) を安全なHTMLに変換する
 * <br> → 改行、<a href> → リンクテキスト、その他タグを除去
 */
function descToSafeHtml(raw) {
  if (!raw) return '';
  return raw
    .replace(/<br\s*\/?>/gi, '\n')
    .replace(/<a[^>]*href="([^"]*)"[^>]*>([^<]*)<\/a>/gi, '$1')
    .replace(/<[^>]+>/g, '')
    .replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;')
    .replace(/"/g,'&quot;').replace(/'/g,'&#39;')
    .replace(/\n/g, '<br>');
}
