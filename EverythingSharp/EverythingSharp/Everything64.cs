using System.Text;
using EverythingSharp.Enums;

namespace EverythingSharp;

public class Everything64 : EverythingBase64
{
    public override IEnumerable<EverythingResult> Search(string query, int maxResults = -1, int offset = -1,
        Sort sort = Sort.NameAscending, RequestFlags requestFlags = RequestFlags.FullPathAndFileName)
    {
        Everything_SetSearchW(query);
        Everything_SetSort((uint)sort);
        Everything_SetRequestFlags((uint)requestFlags);
        if (maxResults > -1)
            Everything_SetMax((uint)maxResults);
        if (offset > -1)
            Everything_SetOffset((uint)offset);

        var success = Everything_QueryW(true);
        if (!success) throw new Exception();

        const int fileAndPathSize = 260;
        var fileAndPathBuffer = new StringBuilder(fileAndPathSize);

        var numResults = Everything_GetNumResults();
        for (uint index = 0; index < numResults; index++)
        {
            fileAndPathBuffer.Clear();
            Everything_GetResultFullPathName(index, fileAndPathBuffer, fileAndPathSize);

            Everything_GetResultSize(index, out var size);
            Everything_GetResultDateCreated(index, out var dateCreated);
            Everything_GetResultDateAccessed(index, out var dateAccessed);
            Everything_GetResultDateModified(index, out var dateModified);
            Everything_GetResultDateRecentlyChanged(index, out var dateRecentlyChanged);
            Everything_GetResultDateRun(index, out var dateRun);

            yield return new EverythingResult
            {
                Size = size,
                FullPath = fileAndPathBuffer.ToString(),
                DateCreated = dateCreated > 0 ? DateTime.FromFileTime(dateCreated) : null,
                DateAccessed = dateAccessed > 0 ? DateTime.FromFileTime(dateAccessed) : null,
                DateModified = dateModified > 0 ? DateTime.FromFileTime(dateModified) : null,
                DateRecentlyChanged = dateRecentlyChanged > 0 ? DateTime.FromFileTime(dateRecentlyChanged) : null,
                DateRun = dateRun > 0 ? DateTime.FromFileTime(dateRun) : null,
                RunCount = Everything_GetResultRunCount(index),
                Attributes = Everything_GetResultAttributes(index)
            };
        }
    }

    public override uint IncrementRunCount(EverythingResult result)
    {
        return Everything_IncRunCountFromFileName(result.FullPath);
    }

    public override void Dispose()
    {
        Everything_Reset();
    }
}