'AG 22/09/2014 -BA-1940
'This TO is designed for use it into current WS results screen
'View results by patient in order to fill properly the patient's list
'Offers:
'1) PatientID / SampleID in patient's list
'2) For each item in 1) a list with the distinct specimenIDs
'3) Row index in presentation grid
'4) PatientID end part of the tooltip string

Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global.TO

    Public Class PatientSpecimenTO

#Region "Attributes"
        Private patientIDAttribute As String 'Patient ID / Sample ID in patient's list (current results patient's list in view results by patient)
        Private specimenIDListAttribute As New List(Of String) 'List of specimen belong this patientID / sample ID
        Private IdRowIndexAttribute As Integer 'Position in control that contains the patientID's list
        Private EndingToolTipTextAttribute As String 'Contains the final part of the tool tip (patient ID code) - names

#End Region

#Region "Attributes"
        Public Property patientID As String
            Get
                Return patientIDAttribute
            End Get
            Set(ByVal Value As String)
                patientIDAttribute = Value
            End Set
        End Property

        Public Property specimenIDList As List(Of String)
            Get
                Return specimenIDListAttribute
            End Get
            'First specimen for this patientID
            Set(Value As List(Of String))
                specimenIDListAttribute.Add(Value(0))
            End Set
        End Property

        Public Property RowIndex As Integer
            Get
                Return IdRowIndexAttribute
            End Get
            Set(Value As Integer)
                IdRowIndexAttribute = value
            End Set
        End Property

        Public Property EndingToolTip As String
            Get
                Return EndingToolTipTextAttribute
            End Get
            Set(value As String)
                EndingToolTipTextAttribute = value
            End Set
        End Property

#End Region

#Region "Update attributes methods"
        Public Function Contains(ByVal pSpecimenID As String) As Boolean
            If Not specimenIDListAttribute.Contains(pSpecimenID) Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Sub UpdateSpecimenList(ByVal pSpecimenID As String)
            If Not specimenIDListAttribute.Contains(pSpecimenID) Then
                specimenIDListAttribute.Add(pSpecimenID)
            End If
        End Sub
#End Region

#Region "Public Methods"

        Public Function GetSpecimenIdListForReports() As String
            Dim specimentList As String = String.Empty

            If (specimenIDList.Count = 1) And (specimenIDList.ElementAt(0) = patientID) Then
                Return String.Empty
            End If

            If (specimenIDList.Count > 0) Then
                specimentList = " ("
                For Each speciment As String In specimenIDList
                    specimentList += String.Format("{0}, ", speciment)
                Next
                specimentList = specimentList.Substring(0, specimentList.Length - 2)
                specimentList += ") "
            End If

            Return specimentList
        End Function

#End Region

#Region "Constructor"
        Public Sub New()
            patientIDAttribute = String.Empty
            specimenIDListAttribute.Clear()
            IdRowIndexAttribute = -1
            EndingToolTipTextAttribute = String.Empty
        End Sub
#End Region

    End Class

End Namespace

