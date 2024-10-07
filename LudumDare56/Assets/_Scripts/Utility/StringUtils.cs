using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUtils
{
    public static string ConvertFloatToMinutesSecondsMilliseconds(float timeSeconds, string separator)
    {
        int minutes = Mathf.FloorToInt(timeSeconds / 60);
        int seconds = Mathf.FloorToInt(timeSeconds - minutes * 60);
        int milliseconds = Mathf.CeilToInt((timeSeconds * 100) % 100);
        
        return $"{minutes:D2}{separator}{seconds:D2}{separator}{milliseconds:D2}";
    }

    public static string FormatPlacing(int place, float placingSuffixSize)
    {
        string placingString = place switch
        {
            1 => $"1<i><size={placingSuffixSize}>st</i></size>",
            2 => $"2<i><size={placingSuffixSize}>nd</i></size>",
            3 => $"3<i><size={placingSuffixSize}>rd</i></size>",
            _ => $"{place}<i><size={placingSuffixSize}>th</i></size>"
        };
        
        return placingString;
    }
}
