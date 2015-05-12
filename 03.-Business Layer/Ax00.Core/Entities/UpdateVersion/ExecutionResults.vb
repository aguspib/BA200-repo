Namespace Biosystems.Ax00.Core.Entities.UpdateVersion

    Public Class ExecutionResults
        Public Property Success As Boolean = True
        Public Property LastErrorRelease As Release
        Public Property LastErrorCommonRevision As CommonRevision
        Public Property LastErrorDataRevision As DataRevision
        Public Property LastErrorStep As ErrorStep = ErrorStep.NoErrors
        Public Enum ErrorStep
            NoErrors
            PrerequisiteScript
            StructureScript
            DataScript
            IntegrityCheckScript
        End Enum
    End Class
End Namespace