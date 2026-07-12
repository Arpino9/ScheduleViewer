const CACHE = 'schedule-viewer-v2';
const STATIC = ['/', '/css/style.css?v=5', '/js/app.js?v=2', '/manifest.json', '/icons/icon.svg'];

self.addEventListener('install', e => {
  e.waitUntil(caches.open(CACHE).then(c => c.addAll(STATIC)));
  self.skipWaiting();
});

self.addEventListener('activate', e => {
  e.waitUntil(
    caches.keys().then(keys =>
      Promise.all(keys.filter(k => k !== CACHE).map(k => caches.delete(k)))
    )
  );
  self.clients.claim();
});

self.addEventListener('fetch', e => {
  const url = new URL(e.request.url);

  // API・ローカル写真は常にネットワーク
  if (url.pathname.startsWith('/api/') || url.pathname.startsWith('/local-photos/')) return;

  // 静的アセット: キャッシュファースト、なければネットワーク取得してキャッシュ
  e.respondWith(
    caches.match(e.request).then(cached => {
      if (cached) return cached;
      return fetch(e.request).then(res => {
        if (res.ok) {
          const clone = res.clone();
          caches.open(CACHE).then(c => c.put(e.request, clone));
        }
        return res;
      });
    })
  );
});
