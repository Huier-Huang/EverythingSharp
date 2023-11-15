using System.Text;
using EverythingSharp.Enums;
using EverythingSharp.Exceptions;
using EverythingSharp.Extensions;

namespace EverythingSharp.Fluent;

public abstract class EverythingSearcher : EverythingBase
{
    public EverythingSearchOptions SearchFor(string query)
    {
        return new EverythingSearchOptions(this).SetQuery(query);
    }

    internal IEnumerable<EverythingEntry> Execute(EverythingSearchOptions options)
    {
        /*
         * Set options
         */
        Everything_SetSearch(options.Query);
        Everything_SetSort((uint)options.Sort);
        Everything_SetRequestFlags((uint)options.Flags);
        if (options.Offset.HasValue)
            Everything_SetOffset(options.Offset.Value);
        if (options.MaxResults.HasValue)
            Everything_SetMax(options.MaxResults.Value);

        /*
         * Perform the search
         */
        var success = Everything_Query(true);
        if (!success)
        {
            var errorCode = (Error)Everything_GetLastError();
            throw new EverythingException(errorCode, errorCode.GetDescription());
        }

        /*
         * Get results
         */
        const int fileAndPathSize = 260;
        var fileAndPathBuffer = new StringBuilder(fileAndPathSize);
        var numResults = Everything_GetNumResults();
        var areAttributesIndexed = Everything_IsFileInfoIndexed((uint)FileInfoType.Attributes);

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

            yield return new EverythingEntry
            {
                Size = size,
                FullPath = fileAndPathBuffer.ToString(),
                DateCreated = dateCreated > 0 ? DateTime.FromFileTime(dateCreated) : null,
                DateAccessed = dateAccessed > 0 ? DateTime.FromFileTime(dateAccessed) : null,
                DateModified = dateModified > 0 ? DateTime.FromFileTime(dateModified) : null,
                DateRecentlyChanged = dateRecentlyChanged > 0 ? DateTime.FromFileTime(dateRecentlyChanged) : null,
                DateRun = dateRun > 0 ? DateTime.FromFileTime(dateRun) : null,
                RunCount = Everything_GetResultRunCount(index),
                Attributes = areAttributesIndexed ? Everything_GetResultAttributes(index) : null,
                Type = Everything_IsFileResult(index) ? EntryType.File :
                    Everything_IsVolumeResult(index) ? EntryType.Volume : EntryType.Folder
            };
        }
    }

    /// <summary>
    ///     Increments the run count for the specified result and returns the new run count.
    /// </summary>
    /// <param name="result">The search result to increase the run counter for.</param>
    /// <returns>The new run count.</returns>
    public uint IncrementRunCount(EverythingEntry result)
    {
        return Everything_IncRunCountFromFileName(result.FullPath);
    }

    /// <summary>
    ///     Increments the run count for the specified path and returns the new run count.
    /// </summary>
    /// <returns>The new run count.</returns>
    public uint IncrementRunCount(string path)
    {
        return Everything_IncRunCountFromFileName(path);
    }

    /// <summary>
    ///     Gets the run count for the given path.
    /// </summary>
    public uint GetRunCountForFile(string path)
    {
        return Everything_GetRunCountFromFileName(path);
    }

    public override void Dispose()
    {
        Everything_CleanUp();
    }
}