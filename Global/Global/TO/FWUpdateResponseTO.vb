Option Explicit On
Option Strict On


Namespace Biosystems.Ax00.Global

    Public Class FWUpdateResponseTO

        Public Sub New()

        End Sub

        Public Sub New(ByVal pActionType As GlobalEnumerates.FwUpdateActions)
            MyClass.ActionTypeAttr = pActionType
        End Sub


        Private ActionTypeAttr As GlobalEnumerates.FwUpdateActions
        Private ActionResultAttr As GlobalEnumerates.FW_GENERIC_RESULT = GlobalEnumerates.FW_GENERIC_RESULT.OK
        Private FirmwareCRCAttr As String = ""
        Private IsUpdatedCPUAttr As GlobalEnumerates.FW_GENERIC_RESULT = GlobalEnumerates.FW_GENERIC_RESULT.KO
        Private IsUpdatedPERAttr As GlobalEnumerates.FW_GENERIC_RESULT = GlobalEnumerates.FW_GENERIC_RESULT.KO
        Private IsUpdatedMANAttr As GlobalEnumerates.FW_GENERIC_RESULT = GlobalEnumerates.FW_GENERIC_RESULT.KO

        Public Property ActionType() As GlobalEnumerates.FwUpdateActions
            Get
                Return ActionTypeAttr
            End Get
            Set(ByVal value As GlobalEnumerates.FwUpdateActions)
                ActionTypeAttr = value
            End Set
        End Property

        Public Property FirmwareCRC() As String
            Get
                Return FirmwareCRCAttr
            End Get
            Set(ByVal value As String)
                FirmwareCRCAttr = value
            End Set
        End Property

        Public Property ActionResult() As GlobalEnumerates.FW_GENERIC_RESULT
            Get
                Return ActionResultAttr
            End Get
            Set(ByVal value As GlobalEnumerates.FW_GENERIC_RESULT)
                ActionResultAttr = value
            End Set
        End Property

        Public Property IsUpdatedCPU() As GlobalEnumerates.FW_GENERIC_RESULT
            Get
                Return IsUpdatedCPUAttr
            End Get
            Set(ByVal value As GlobalEnumerates.FW_GENERIC_RESULT)
                IsUpdatedCPUAttr = value
            End Set
        End Property

        Public Property IsUpdatedPER() As GlobalEnumerates.FW_GENERIC_RESULT
            Get
                Return IsUpdatedPERAttr
            End Get
            Set(ByVal value As GlobalEnumerates.FW_GENERIC_RESULT)
                IsUpdatedPERAttr = value
            End Set
        End Property


        Public Property IsUpdatedMAN() As GlobalEnumerates.FW_GENERIC_RESULT
            Get
                Return IsUpdatedMANAttr
            End Get
            Set(ByVal value As GlobalEnumerates.FW_GENERIC_RESULT)
                IsUpdatedMANAttr = value
            End Set
        End Property



    End Class

End Namespace
