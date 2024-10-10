// 基本
global using Newtonsoft.Json.Linq;
global using System;
global using System.Collections.Generic;
global using System.Data.SQLite;
global using System.Drawing;
global using System.Linq;
global using System.IO;
global using System.Net;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Xml.Serialization;
global using System.Windows.Media;
global using System.Windows.Media.Imaging;

// API
global using Google.Apis.Auth.OAuth2;
global using Google.Apis.Books.v1;
global using Google.Apis.Calendar.v3;
global using Google.Apis.Calendar.v3.Data;
global using Google.Apis.Services;
global using Google.Apis.MapsPlaces.v1;
global using Newtonsoft.Json;

// Domain層
global using ScheduleViewer.Domain;
global using ScheduleViewer.Domain.Entities;
global using ScheduleViewer.Domain.Exceptions;
global using ScheduleViewer.Domain.Modules.Helpers;
global using ScheduleViewer.Domain.Modules.Logics;
global using ScheduleViewer.Domain.Repositories;

// Infrastructure層
global using ScheduleViewer.Infrastructure.GoogleService;
global using ScheduleViewer.Infrastructure.Google_Calendar;
global using ScheduleViewer.Infrastructure.Google_Drive;
global using ScheduleViewer.Infrastructure.Google_Fitness;
global using ScheduleViewer.Infrastructure.Google_Photo;
global using ScheduleViewer.Infrastructure.Google_MapsPlace;
global using ScheduleViewer.Infrastructure.Google_SpreadSheet;
global using ScheduleViewer.Infrastructure.Google_Tasks;
global using ScheduleViewer.Infrastructure.JSON;
global using ScheduleViewer.Infrastructure.XML;

// API
global using Google.Apis.Drive.v3;
global using Google.Apis.Download;
global using Google.Apis.Fitness.v1;
global using Google.Apis.Fitness.v1.Data;
global using Google.Apis.PhotosLibrary.v1;
global using Google.Apis.PhotosLibrary.v1.Data;
global using Google.Apis.Tasks.v1;
global using Google.Apis.Util.Store;
global using Google.Apis.Sheets.v4;