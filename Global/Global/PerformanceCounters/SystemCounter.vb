'What is the correct Performance Counter to get CPU and Memory Usage of a Process?
'http://stackoverflow.com/questions/4679962/what-is-the-correct-performance-counter-to-get-cpu-and-memory-usage-of-a-process

'How to fix error CPU usage is always zero
'Performance Counter are always zero in vb.net
'http://stackoverflow.com/questions/19006480/performance-counter-are-always-zero-in-vb-net

'Performance counter definition
'http://technet.microsoft.com/en-us/library/aa997156(v=exchg.65).aspx


Option Explicit On

Imports System.Diagnostics

Namespace Biosystems.Ax00.Global

    ''' <summary>
    ''' Created by: AG + JV 07/02/2014 BT #1499
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class SystemCounter

#Region "Definitions"

        Public Enum CountersMode
            Computer
            Process
        End Enum


        Public Enum CountersType
            Computer_UsageCPU
            Computer_AvailableMemory
            Process_UsageCPU
            Process_WorkingSet 'Memory usage
            Process_VirtualBytes
            Process_PrivateBytes
            'jv
            Handle_Count
            Page_File_Bytes
            Available_MBytes
            Pool_Nonpaged_Bytes
            Pool_Paged_Bytes
            Committed_Bytes
            Bytes_in_all_Heaps
            Large_Object_Heap_size
            Gen_2_heap_size
            Gen_1_heap_size
            Gen_0_heap_size
        End Enum

        Private theCounterAttribute As New System.Diagnostics.PerformanceCounter
        Private theProcessNameAttribute As String = Process.GetCurrentProcess.ProcessName
        Private theErrorAttribute As Boolean = False
        Private theDivisorFactor As Single = 1

#End Region

#Region "Properties"

        Public ReadOnly Property HasError As Boolean
            Get
                Return theErrorAttribute
            End Get
        End Property

        'jv
        Public ReadOnly Property Name As String
            Get
                Return theCounterAttribute.CounterName
            End Get
        End Property

#End Region

#Region "Constructor"


        ''' <summary>
        ''' Counters en inglés siempre en el registry: http://stackoverflow.com/questions/11538299/access-windows-performance-counters-in-a-locale-independent-way
        ''' </summary>
        ''' <param name="pType"></param>
        ''' <param name="pProcessName"></param>
        ''' <param name="pDivisorFactor"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal pType As CountersType, Optional ByVal pProcessName As String = "", Optional pDivisorFactor As Single = 1)
            Try
                If pProcessName <> "" Then theProcessNameAttribute = pProcessName
                If pDivisorFactor <> 1 Then theDivisorFactor = pDivisorFactor

                Select Case pType
                    Case CountersType.Computer_UsageCPU
                        theCounterAttribute = New PerformanceCounter("Processor", "% Processor Time", "_Total") '("Procesador", "% de tiempo de procesador", "_Total") 
                    Case CountersType.Process_UsageCPU
                        theCounterAttribute = New PerformanceCounter("Process", "% Processor Time", theProcessNameAttribute)
                    Case CountersType.Process_WorkingSet
                        theCounterAttribute = New PerformanceCounter("Process", "Working Set", theProcessNameAttribute) 'Espacio de trabajo
                    Case CountersType.Process_VirtualBytes
                        theCounterAttribute = New PerformanceCounter("Process", "Virtual bytes", theProcessNameAttribute)
                    Case CountersType.Process_PrivateBytes
                        theCounterAttribute = New PerformanceCounter("Process", "Private bytes", theProcessNameAttribute)
                        'jv
                    Case CountersType.Handle_Count
                        theCounterAttribute = New PerformanceCounter("Process", "Handle Count", theProcessNameAttribute)
                    Case CountersType.Page_File_Bytes
                        theCounterAttribute = New PerformanceCounter("Process", "Page File Bytes", theProcessNameAttribute)
                    Case CountersType.Available_MBytes
                        theCounterAttribute = New PerformanceCounter("Memory", "Available MBytes")
                    Case CountersType.Pool_Nonpaged_Bytes
                        theCounterAttribute = New PerformanceCounter("Memory", "Pool Nonpaged Bytes")
                    Case CountersType.Pool_Paged_Bytes
                        theCounterAttribute = New PerformanceCounter("Memory", "Pool Paged Bytes")
                    Case CountersType.Committed_Bytes
                        theCounterAttribute = New PerformanceCounter("Memory", "Committed Bytes")
                    Case CountersType.Bytes_in_all_Heaps
                        theCounterAttribute = New PerformanceCounter(".NET CLR Memory", "# Bytes in all Heaps", theProcessNameAttribute)
                    Case CountersType.Large_Object_Heap_size
                        theCounterAttribute = New PerformanceCounter(".NET CLR Memory", "Large Object Heap size", theProcessNameAttribute)
                    Case CountersType.Gen_2_heap_size
                        theCounterAttribute = New PerformanceCounter(".NET CLR Memory", "Gen 2 heap size", theProcessNameAttribute)
                    Case CountersType.Gen_1_heap_size
                        theCounterAttribute = New PerformanceCounter(".NET CLR Memory", "Gen 1 heap size", theProcessNameAttribute)
                    Case CountersType.Gen_0_heap_size
                        theCounterAttribute = New PerformanceCounter(".NET CLR Memory", "Gen 0 heap size", theProcessNameAttribute)
                    Case Else
                End Select
            Catch ex As Exception
                theErrorAttribute = True
            End Try
        End Sub

#End Region

#Region "Public Methods"

        Public Function NextValue() As Single
            Try
                If theDivisorFactor = 0 Then theDivisorFactor = 1
                Return (theCounterAttribute.NextValue / theDivisorFactor)
            Catch ex As Exception
                theErrorAttribute = True
            End Try
        End Function

#End Region

    End Class

End Namespace
