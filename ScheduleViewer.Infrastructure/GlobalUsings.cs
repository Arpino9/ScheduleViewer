﻿// 基本
global using System;
global using System.Collections.Generic;
global using System.Data.SQLite;
global using System.Drawing;
global using System.Linq;
global using System.IO;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Xml.Serialization;
global using System.Windows.Media;

// API
global using Google.Apis.Auth.OAuth2;
global using Google.Apis.Calendar.v3;
global using Google.Apis.Calendar.v3.Data;
global using Google.Apis.Services;
global using Newtonsoft.Json;

// Domain層
global using ScheduleViewer.Domain;
global using ScheduleViewer.Domain.Entities;
global using ScheduleViewer.Domain.Exceptions;
global using ScheduleViewer.Domain.Modules.Helpers;
global using ScheduleViewer.Domain.Modules.Logics;
global using ScheduleViewer.Domain.Repositories;

// Infrastructure層
global using ScheduleViewer.Infrastructure.XML;

// API
global using Google.Apis.Util.Store;