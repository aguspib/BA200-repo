Imports System.IO
Imports NUnit.Framework

Imports Logger


Namespace Logger.Tests

    <TestFixture()> Public Class MessageLoggerTests

        ''' <summary>
        ''' Test the header's file and ctor of logger
        ''' </summary>
        <Test()> Public Sub MessageLogger_CreateNewInstance_ParameterizedValuesInicializated()
            Dim messageLog As New MessageLogger("LogConsum", "AnalyzerSN;TestName;SampleClass;SampleType;VR1(uL);BarcodeR1;VR2(uL);BarcodeR2", "CtorTest_Preparation")

            Assert.AreEqual(messageLog.FullHeaderFile, "Date;AnalyzerSN;TestName;SampleClass;SampleType;VR1(uL);BarcodeR1;VR2(uL);BarcodeR2")
            Assert.AreEqual(messageLog.FolderName, "LogConsum\")
            Assert.AreEqual(messageLog.PrefixFile, "CtorTest_Preparation")

        End Sub

        ''' <summary>
        ''' Test the creation of the file, add line and clean after test
        ''' </summary>
        <Test()> Public Sub MessageLogger_AddLog_CreateNewFileMonthlyWithHeaderAndNewLine()
            Dim messageLog As New MessageLogger("LogConsum", "AnalyzerSN;TestName;SampleClass;SampleType;VR1(uL);BarcodeR1;VR2(uL);BarcodeR2", "CreateNewTest_Preparation", MessageLogger.LogFrequency.Monthly)

            messageLog.AddLog("Ax00_000000;Patient;SER;xx;;xx;")

            Threading.Thread.Sleep(1000)

            Assert.IsTrue(File.Exists(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "CreateNewTest_Preparation_" & Date.Now.ToString("yyyy_MM") & ".csv")))

            'Cleaning
            File.Delete(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "CreateNewTest_Preparation_" & Date.Now.ToString("yyyy_MM") & ".csv"))
            Assert.IsFalse(File.Exists(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "CreateNewTest_Preparation_" & Date.Now.ToString("yyyy_MM") & ".csv")))

        End Sub

        ''' <summary>
        ''' Test the creation of the file, add line and clean after test
        ''' </summary>
        <Test()> Public Sub MessageLogger_AddLog_CreateNewFileDailyWithHeaderAndNewLine()
            Dim messageLog As New MessageLogger("LogConsum", "AnalyzerSN;TestName;SampleClass;SampleType;VR1(uL);BarcodeR1;VR2(uL);BarcodeR2", "CreateNewTest_Preparation", MessageLogger.LogFrequency.Daily)

            messageLog.AddLog("Ax00_000000;Patient;SER;xx;;xx;")

            Threading.Thread.Sleep(1000)

            Assert.IsTrue(File.Exists(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "CreateNewTest_Preparation_" & Date.Now.ToString("yyyy_MM_dd") & ".csv")))

            'Cleaning
            File.Delete(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "CreateNewTest_Preparation_" & Date.Now.ToString("yyyy_MM_dd") & ".csv"))
            Assert.IsFalse(File.Exists(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "CreateNewTest_Preparation_" & Date.Now.ToString("yyyy_MM_dd") & ".csv")))

        End Sub

        ''' <summary>
        ''' Test the creation of the file, add more than one line (accumulation) and clean after test
        ''' </summary>
        <Test()> Public Sub MessageLogger_AddLog_CreateNewFileWithHeaderAndFewLines()
            Dim messageLog As New MessageLogger("LogConsum", "AnalyzerSN;PreparationDate;TestName;SampleClass;SampleType;VR1(uL);BarcodeR1;VR2(uL);BarcodeR2", "AddLinesTest_Preparation", MessageLogger.LogFrequency.Monthly)

            messageLog.AddLog("Ax00_000000;" & Date.Now.ToString("yyyy/MM/dd") & ";Patient1;SER;xx;;xx;")

            messageLog.AddLog("Ax00_000000;" & Date.Now.ToString("yyyy/MM/dd") & ";Patient1;URI;xx;;xx;")

            messageLog.AddLog("Ax00_000000;" & Date.Now.ToString("yyyy/MM/dd") & ";Patient2;SER;xx;;xx;")

            messageLog.AddLog("Ax00_000000;" & Date.Now.ToString("yyyy/MM/dd") & ";Patient2;URI;xx;;xx;")

            Threading.Thread.Sleep(1000)

            Assert.IsTrue(File.Exists(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "AddLinesTest_Preparation_" & Date.Now.ToString("yyyy_MM") & ".csv")))
            Assert.AreEqual(5, File.ReadAllLines(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "AddLinesTest_Preparation_" & Date.Now.ToString("yyyy_MM") & ".csv")).Length)

            'Cleaning
            File.Delete(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "AddLinesTest_Preparation_" & Date.Now.ToString("yyyy_MM") & ".csv"))
            Assert.IsFalse(File.Exists(Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "AddLinesTest_Preparation_" & Date.Now.ToString("yyyy_MM") & ".csv")))

        End Sub

        ''' <summary>
        ''' Test the creation of the file, add 25000 lines in the same file
        ''' </summary>
        <Test()> Public Sub MessageLogger_AddLog_BulkWriteLines()

            Dim messageLog As New MessageLogger("LogConsum", "AnalyzerSN;TestName;SampleClass;SampleType;VR1(uL);BarcodeR1;VR2(uL);BarcodeR2", "BulkTest_Preparation", MessageLogger.LogFrequency.Monthly)

            Dim filePath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\BioSystemsDev\" & messageLog.FolderName & "BulkTest_Preparation_" & Date.Now.ToString("yyyy_MM") & ".csv")
            If IO.File.Exists(filePath) Then
                Try
                    File.Delete(filePath)
                Catch : End Try
            End If


            Dim bulkCounter = 0

            While bulkCounter < 25000
                messageLog.AddLog("Ax00_000000;Patient" & bulkCounter.ToString & ";CALIB;SER;;;;")
                bulkCounter += 1
            End While

            While messageLog.QueuedItems > 0
                Dim t = Task.Delay(1000)
                t.Wait()
            End While
            Debug.WriteLine("done, checking correctness...")

            Assert.IsTrue(File.Exists(filePath))
            Assert.AreEqual(25001, File.ReadAllLines(filePath).Length)

            'Cleaning
            File.Delete(filePath)
            Assert.IsFalse(IO.File.Exists(filePath))

        End Sub
    End Class


End Namespace


