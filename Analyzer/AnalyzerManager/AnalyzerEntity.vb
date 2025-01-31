﻿Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.Data
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Partial Public MustInherit Class AnalyzerManager
        Implements IAnalyzerManager




        Private WithEvents _baseLine As IBaseLineEntity
        Private WithEvents _iseAnalyzer As IISEManager

#Region "Properties"

        Property BaseLine As IBaseLineEntity Implements IAnalyzerManager.BaseLine
            Get
                Return _baseLine
            End Get
            Set(value As IBaseLineEntity)
                _baseLine = value
            End Set
        End Property

        Property ISEAnalyzer As IISEManager Implements IAnalyzerManager.ISEAnalyzer
            Get
                Return _iseAnalyzer
            End Get
            Set(value As IISEManager)
                _iseAnalyzer = value
            End Set
        End Property

        Property BaseLineTypeForCalculations As BaseLineType Implements IAnalyzerManager.BaseLineTypeForCalculations
        Property BaseLineTypeForWellReject As BaseLineType Implements IAnalyzerManager.BaseLineTypeForWellReject

        Property Model As String Implements IAnalyzerManager.Model
            Get
                If myAnalyzerModel = "" Then
                    Return GetModelValue(GenericDefaultAnalyzer)
                End If
                Return myAnalyzerModel
            End Get
            Set(value As String)
                myAnalyzerModel = value
            End Set
        End Property

#End Region

        'AG 30/10/2014 BA-2064 comment new code temporally
        'Property Calculations As CalculationsDelegate Implements IAnalyzerEntity.calculations

        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity)
            Me.New(assemblyName, analyzerModel)
            _baseLine = baseLine
        End Sub


#Region "Abstract methods"
        ''' <summary>
        ''' Get the urrent (last) base line ID. It must be override because different model may use a different type of base line (STATIC or DYNAMIC)
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pWell"></param>
        ''' <param name="pBaseLineWithAdjust"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetCurrentBaseLineID(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String, ByVal pWell As Integer, ByVal pBaseLineWithAdjust As Boolean) As GlobalDataTO

        Public MustOverride Function ExistBaseLineFinished() As Boolean Implements IAnalyzerManager.ExistBaseLineFinished
        Public MustOverride Function BaseLineNotStarted() As Boolean Implements IAnalyzerManager.BaseLineNotStarted

#End Region

        'Public MustOverride Function ContaminationsSpecification() As IAnalyzerContaminationsSpecification Implements IAnalyzerManager.ContaminationsSpecification

        Public MustOverride ReadOnly Property WashingIDRequired As Boolean Implements IAnalyzerManager.WashingIDRequired


        Public MustOverride ReadOnly Property FirmwareFileExtension As String Implements IAnalyzerManager.FirmwareFileExtension

        Public MustOverride Function CommercialModelName() As String Implements IAnalyzerManager.CommercialModelName
    End Class
End Namespace
