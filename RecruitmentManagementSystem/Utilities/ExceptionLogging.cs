namespace RecruitmentManagementSystem.Utilities;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

public static class ExceptionLogging
{
    public static void SendErrorToText(Exception ex, HttpContext context)
    {
        string errorLineNo = "";
        string errorMsg = "";
        string exType = "";
        string exUrl = "";
        string errorLocation = "";
        var lineBreak = Environment.NewLine + Environment.NewLine;

        try
        {
            Console.WriteLine($"[DEBUG] Method SendErrorToText invoked at: {DateTime.Now}");

            if (ex.StackTrace != null && ex.StackTrace.Length > 7)
            {
                errorLineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
                Console.WriteLine($"[DEBUG] Extracted Error Line No: {errorLineNo}");
            }

            errorMsg = ex.Message;
            exType = ex.GetType().ToString();
            exUrl = context?.Request?.Path ?? "N/A";
            errorLocation = ex.StackTrace ?? "N/A";

            Console.WriteLine($"[DEBUG] Error Details Captured: {errorMsg}, {exType}, {exUrl}");

            // Set the file path
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExceptionDetailsFile");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
                Console.WriteLine($"[DEBUG] Created log directory: {logDirectory}");
            }

            string logFilePath = Path.Combine(logDirectory, DateTime.Today.ToString("dd-MM-yyyy") + ".txt");
            Console.WriteLine($"[DEBUG] Log file path: {logFilePath}");

            // Ensure new log file is created each day
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                string errorDetails = $"Log Written Date: {DateTime.Now}{lineBreak}" +
                                      $"Error Line No: {errorLineNo}{lineBreak}" +
                                      $"Error Message: {errorMsg}{lineBreak}" +
                                      $"Exception Type: {exType}{lineBreak}" +
                                      $"Error Location: {errorLocation}{lineBreak}" +
                                      $"Error Page URL: {exUrl}{lineBreak}";

                sw.WriteLine("----------- Exception Details -----------");
                sw.WriteLine(errorDetails);
                sw.WriteLine("----------------------------------------");
            }

            Console.WriteLine($"[DEBUG] Exception details successfully written to file.");
        }
        catch (Exception loggingEx)
        {
            Console.WriteLine($"[DEBUG] Error while logging exception: {loggingEx.Message}");
        }
    }

}
