import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

export enum LogLevel {
  All = 0,
  Debug = 1,
  Info = 2,
  Warn = 3,
  Error = 4,
  Fatal = 5,
  Off = 6
}

@Injectable({
  providedIn: 'root'
})
export class LoggerService {
  level: LogLevel = environment.logLevel;

  private log(level: LogLevel, message: any, ...optionalParams: any[]) {
    if (this.shouldLog(level)) {
      const timestamp = new Date().toISOString();
      const logMessage = `[${LogLevel[level]}] [${timestamp}] ${message}`;

      switch (level) {
        case LogLevel.Warn:
          console.warn(logMessage, ...optionalParams);
          break;
        case LogLevel.Error:
        case LogLevel.Fatal:
          console.error(logMessage, ...optionalParams);
          break;
        case LogLevel.Info:
          console.info(logMessage, ...optionalParams);
          break;
        default:
          console.log(logMessage, ...optionalParams);
          break;
      }
    }
  }

  private shouldLog(level: LogLevel): boolean {
    return level >= this.level;
  }

  debug(message: any, ...optionalParams: any[]) {
    this.log(LogLevel.Debug, message, ...optionalParams);
  }

  info(message: any, ...optionalParams: any[]) {
    this.log(LogLevel.Info, message, ...optionalParams);
  }

  warn(message: any, ...optionalParams: any[]) {
    this.log(LogLevel.Warn, message, ...optionalParams);
  }

  error(message: any, ...optionalParams: any[]) {
    this.log(LogLevel.Error, message, ...optionalParams);
  }

  fatal(message: any, ...optionalParams: any[]) {
    this.log(LogLevel.Fatal, message, ...optionalParams);
  }
}
