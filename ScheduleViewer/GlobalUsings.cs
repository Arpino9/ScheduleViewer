﻿// 基本
global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Globalization;
global using System.Linq;
global using System.IO;
global using System.Reactive.Linq;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Data;
global using System.Windows.Forms;
global using System.Windows.Media;
global using System.Windows.Input;

// Domain層
global using ScheduleViewer.Domain;
global using ScheduleViewer.Domain.Entities;
global using ScheduleViewer.Domain.Exceptions;
global using ScheduleViewer.Domain.Modules.Logics;
global using ScheduleViewer.Domain.Modules.Helpers;
global using ScheduleViewer.Domain.Repositories;
global using ScheduleViewer.Domain.StaticValues;
global using ScheduleViewer.Domain.ValueObjects;

// View層
global using ScheduleViewer.ViewModels;

// API
global using Reactive.Bindings;