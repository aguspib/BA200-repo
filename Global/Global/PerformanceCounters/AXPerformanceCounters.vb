
Imports System.IO
Imports System.Text

Namespace Biosystems.Ax00.Global

    ''' <summary>
    ''' Created by: AG + JV 07/02/2014 BT #1499
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AXPerformanceCounters

#Region "Declarations"

        Private Const convertToMegaBytes As Single = 1000000

        'Declare here, not in event (because then the CPU counter returns always 0)
        'http://stackoverflow.com/questions/19006480/performance-counter-are-always-zero-in-vb-net
        'Info: http://msdn.microsoft.com/es-es/library/system.diagnostics.process.privatememorysize64(v=vs.110).aspx


        'AG 24/02/2014 - #1520
        Private maxAppMemoryUsage As Single = 900 'LIMIT MAX (in Mbytes) for private bytes for BA400 application (initialized in constructior)
        Private maxSQLMemoryUsage As Single = 1000 'LIMIT MAX (in Mbytes) for private bytes for SQL service (initialized in constructior)
        Private limitExceededFlag As Boolean = False
        'AG 24/02/2014 - #1520

#End Region

#Region "Properties"
        'AG 24/02/2014 - #1520
        Public ReadOnly Property LimitExceeded As Boolean
            Get
                'Return limitExceededFlag
                Return False
            End Get
        End Property
        'AG 24/02/2014 - #1520

#End Region

#Region "Constructor"
        'AG 24/02/2014 - #1520
        'Limits must be informed as parameters because Global cannot call the DAO methods (circular references error)
        Public Sub New(ByVal pAppMaxMemoryUsage As Single, ByVal pSQLMaxMemoryUsage As Single)
            If pAppMaxMemoryUsage > 0 Then
                maxAppMemoryUsage = pAppMaxMemoryUsage
            End If
            If pSQLMaxMemoryUsage > 0 Then
                maxSQLMemoryUsage = pSQLMaxMemoryUsage
            End If
        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Get the memory processes usage and other performance counters. Log the values as required.
        ''' CREATED BY:  AG + JV 07/02/2014 BT #1499
        ''' MODIFIED BY: JV 10/02/2014 BT #1499 - Include the processes memory usage as required. Also do not use the convertToMegaBytes value. Don´t put every performance counter in a different log-row (all in the same row) 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub GetAllCounters()

            Dim value As Single = 0
            Dim myOthers As New List(Of SystemCounter)
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                Dim location As String = Environment.GetCommandLineArgs()(0)
                Dim myProcessName As String = (Path.GetFileName(location)).Replace(".exe", String.Empty)

                'Mem. usage:
                Dim myDictionary As Dictionary(Of Process, Long) = Process.GetProcesses().ToDictionary(Of Process, Long)(Function(x) x, Function(y) y.PrivateMemorySize64)
                Dim myMax As KeyValuePair(Of Process, Long) = myDictionary.FirstOrDefault(Function(x) x.Value = myDictionary.Select(Function(y) y.Value).Max())

                Dim myTxt As New StringBuilder
                myTxt.Append("Max. Value (Bytes): " & myMax.Key.ProcessName & " = " & myMax.Value.ToString("N0") & " / ")
                Dim myTemp As KeyValuePair(Of Process, Long) = myDictionary.FirstOrDefault(Function(x) x.Key.ProcessName = myProcessName)
                If Not myTemp.Key Is Nothing Then myTxt.Append("BA400 = " & myTemp.Value.ToString("N0") & " / ")
                If Not limitExceededFlag AndAlso CSng((myTemp.Value) / convertToMegaBytes) > maxAppMemoryUsage Then
                    limitExceededFlag = True 'AG 24/02/2014 - #1520 - Check if max limit is exceeded for the BA400 memory usage counter
                    GlobalBase.CreateLogActivity("Performance message: Memory BA400 > MAX_APP_MEMORYUSAGE", "AXPerformanceCounters.GetAllCounters", EventLogEntryType.Information, False) 'AG 27/02/2014 - add trace
                End If



                myTemp = myDictionary.FirstOrDefault(Function(x) x.Key.ProcessName = "sqlservr")
                If Not myTemp.Key Is Nothing Then myTxt.Append("sqlservr = " & myTemp.Value.ToString("N0") & " / ")
                If Not limitExceededFlag AndAlso CSng((myTemp.Value) / convertToMegaBytes) > maxSQLMemoryUsage Then
                    limitExceededFlag = True 'AG 24/02/2014 - #1520 - Check if max limit is exceeded for the SQL memory usage counter
                    GlobalBase.CreateLogActivity("Performance message: Memory SQLserv > MAX_SQL_MEMORYUSAGE", "AXPerformanceCounters.GetAllCounters", EventLogEntryType.Information, False) 'AG 27/02/2014 - add trace
                End If



                myTemp = myDictionary.FirstOrDefault(Function(x) x.Key.ProcessName = "sqlbrowser")
                If Not myTemp.Key Is Nothing Then myTxt.Append("sqlbrowser = " & myTemp.Value.ToString("N0") & " / ")
                'AG 27/02/2014 - CANCELLED, not check this service
                'If Not limitExceededFlag AndAlso CSng((myTemp.Value) / convertToMegaBytes) > maxSQLMemoryUsage Then limitExceededFlag = True 'AG 24/02/2014 - #1520 - Check if max limit is exceeded for the SQL memroy usage counter


                myTemp = myDictionary.FirstOrDefault(Function(x) x.Key.ProcessName = "sqlwriter")
                If Not myTemp.Key Is Nothing Then myTxt.Append("sqlwriter = " & myTemp.Value.ToString("N0"))
                'AG 27/02/2014 - CANCELLED, not check this service
                'If Not limitExceededFlag AndAlso CSng((myTemp.Value) / convertToMegaBytes) > maxSQLMemoryUsage Then limitExceededFlag = True 'AG 24/02/2014 - #1520 - Check if max limit is exceeded for the SQL memroy usage counter


                GlobalBase.CreateLogActivity(myTxt.ToString(), "AXPerformanceCounters.GetAllCounters", EventLogEntryType.Information, False)

                'Performance counters:
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Available_MBytes, myProcessName)) 'Available memory
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Committed_Bytes, myProcessName, convertToMegaBytes)) 'Commited bytes
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Process_PrivateBytes, myProcessName, convertToMegaBytes)) 'Private bytes
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Page_File_Bytes, myProcessName, convertToMegaBytes)) 'page file bytes
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Handle_Count, myProcessName)) 'handle count
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Bytes_in_all_Heaps, myProcessName, convertToMegaBytes)) 'bytes in all heaps
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Large_Object_Heap_size, myProcessName, convertToMegaBytes)) 'large objects heap size
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Gen_2_heap_size, myProcessName, convertToMegaBytes))
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Gen_1_heap_size, myProcessName, convertToMegaBytes))
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Gen_0_heap_size, myProcessName, convertToMegaBytes))
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Process_VirtualBytes, myProcessName, convertToMegaBytes))

                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Available_MBytes, myProcessName)) 'Available memory
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Committed_Bytes, myProcessName)) 'Commited bytes
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Process_PrivateBytes, myProcessName)) 'Private bytes
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Page_File_Bytes, myProcessName)) 'page file bytes
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Handle_Count, myProcessName)) 'handle count
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Bytes_in_all_Heaps, myProcessName)) 'bytes in all heaps
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Large_Object_Heap_size, myProcessName)) 'large objects heap size
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Gen_2_heap_size, myProcessName))
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Gen_1_heap_size, myProcessName))
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Gen_0_heap_size, myProcessName))
                myOthers.Add(New SystemCounter(SystemCounter.CountersType.Process_VirtualBytes, myProcessName))

                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Computer_UsageCPU))
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Process_UsageCPU, myProcessName))
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Process_WorkingSet, myProcessName, convertToMegaBytes))
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Pool_Nonpaged_Bytes, myProcessName))
                'myOthers.Add(New SystemCounter(SystemCounter.CountersType.Pool_Paged_Bytes, myProcessName))

                myTxt.Length = 0
                myTxt.Append("BA400 Counters ")
                For Each counter As SystemCounter In myOthers
                    value = counter.NextValue
                    If Not counter.HasError Then
                        myTxt.Append(counter.Name & ": " & value.ToString("N0") & " / ")
                    Else
                        myTxt.Append(counter.Name & ": NoInfo" & " / ") 'myTxt.Append(counter.Name & ": Error" & " / ")
                    End If
                Next
                GlobalBase.CreateLogActivity(myTxt.ToString(), "AXPerformanceCounters.GetAllCounters", EventLogEntryType.Information, False)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AXPerformanceCounters.GetAllCounters", EventLogEntryType.Error, False)
            End Try

        End Sub

#End Region

    End Class

End Namespace