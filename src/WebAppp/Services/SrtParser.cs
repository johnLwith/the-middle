using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class SubtitleEntry
{
    public int SequenceNumber { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public List<string> Lines { get; set; } = new List<string>();
}

public class SrtParser
{
    public static List<SubtitleEntry> ParseSrtFile(string content)
    {
        var entries = new List<SubtitleEntry>();
        var lines = content.Split(Environment.NewLine);

        SubtitleEntry currentEntry = null;
        var state = 0; // 0: expecting sequence number, 1: expecting timestamp, 2: expecting text

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (currentEntry != null)
                {
                    entries.Add(currentEntry);
                    currentEntry = null;
                }
                state = 0;
                continue;
            }

            if (state == 0 && int.TryParse(line, out int sequenceNumber))
            {
                currentEntry = new SubtitleEntry { SequenceNumber = sequenceNumber };
                state = 1;
            }
            else if (state == 1 && line.Contains("-->"))
            {
                var timeMatch = Regex.Match(line, @"(\d{2}:\d{2}:\d{2},\d{3})\s*-->\s*(\d{2}:\d{2}:\d{2},\d{3})");
                if (timeMatch.Success)
                {
                    currentEntry.StartTime = timeMatch.Groups[1].Value;
                    currentEntry.EndTime = timeMatch.Groups[2].Value;
                    state = 2;
                }
            }
            else if (state == 2)
            {
                currentEntry.Lines.Add(line);
            }
        }

        // Add the last entry if it exists
        if (currentEntry != null)
        {
            entries.Add(currentEntry);
        }

        return entries;
    }
}