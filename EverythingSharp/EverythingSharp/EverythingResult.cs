﻿namespace EverythingSharp;

public class EverythingResult
{
    public long Size { get; internal set; }
    public string FullPath { get; internal set; }
    public DateTime? DateCreated { get; internal set; }
    public DateTime? DateAccessed { get; internal set; }
    public DateTime? DateModified { get; internal set; }
    public DateTime? DateRecentlyChanged { get; internal set; }
    public DateTime? DateRun { get; internal set; }
    public uint RunCount { get; internal set; }
    public uint Attributes { get; internal set; }
}