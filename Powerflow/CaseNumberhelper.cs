using System.Reflection;
using static PsseCase;

public static class CaseNumberHelper
{
    public static void SetCaseReference(object entity, int caseNum, CaseInfo caseInfo)
    {
        var type = entity.GetType();

        // Set the foreign key property (CaseNumber)
        var caseNumberProp = type.GetProperty("CaseNumber", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (caseNumberProp != null && caseNumberProp.CanWrite && caseNumberProp.PropertyType == typeof(int))
        {
            caseNumberProp.SetValue(entity, caseNum);
        }

        // Set the navigation property (CaseNum)
        var caseInfoProp = type.GetProperty("CaseNum", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (caseInfoProp != null && caseInfoProp.CanWrite && caseInfoProp.PropertyType == typeof(CaseInfo))
        {
            caseInfoProp.SetValue(entity, caseInfo);
        }
    }

}

