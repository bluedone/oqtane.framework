﻿using Oqtane.Models;
using Oqtane.Shared;
using System;

namespace Oqtane.Infrastructure
{
    public interface ILogManager
    {
        void Log(LogLevel Level, string Category, LogFunction Function, string Message, params object[] Args);
        void Log(LogLevel Level, string Category, LogFunction Function, Exception Exception, string Message, params object[] Args);
        void Log(Log Log);
    }
}
