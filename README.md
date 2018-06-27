Opening Hours
===

Helper classes for showing if something is open or closed by using a content type stored in a Contensis CMS.

Setup in the CMS
----

To use requires the following to be set up in the CMS:

*   Taxomony for open time period types (these could be buildings, shops or telephone lines)
*   Day Open Times component
*   Open Time Period content type

### Day Open Times component

| Name    | Type    | Required | Settings      |
| --------| ------- | ---------|-------------- |
| Days    | List    | Yes      | Allow mulitple: Yes<br>List values: "Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday" |
| Start   | Text(1) | Yes      | Format: Text<br>MATCHES PATTERN: 24 hour time |
| End     | Text(1) | Yes      | Format: Text<br>MATCHES PATTERN: 24 hour time |

<a href="1">1</a> If there was a 'time' type we would use that

![Screenshot of the Day Open Times component](https://raw.githubusercontent.com/UniversityOfBrightonComputing/Contensis.OpeningHours/master/DayOpenTimes.png)

### Open Time Period content type

This content type has the form:

| Name       | Type      | Required | Settings      |
| -----------| ----------| ---------|-------------- |
| Name       | Text      | Yes      |  |
| Period for | Taxonomy  | Yes      | Allow multiple: Yes<br>Minimum 1 item |
| When       | Date/time | Yes      | Format: Range<br>Editor: Date Range |
| Priority   | Number    | Yes      | Format: Integer<br>Min: 1<br>Max: 5 |
| Day Open Times | Component (Day Open Times) | N/A | Allow multiple: Yes
| Exceptions | Date/time | No       | Format: Single<br>Allow multiple: Yes |

![Screenshot of the Open Time Period content type](https://raw.githubusercontent.com/UniversityOfBrightonComputing/Contensis.OpeningHours/master/OpenTimePeriod.png)


Installation
---

### .Net App

Via Nuget: https://www.nuget.org/packages/UniversityOfBrighton.Contensis.OpeningHours/

### On CMS

Either download from https://www.nuget.org/packages/UniversityOfBrighton.Contensis.OpeningHours/ and unzip and add as Custom DLL to the site

Or copy into App_code folder the following files (in this order)

1. UniversityOfBrighton.Contensis.OpeningHours\DayOpenTime.cs
2. UniversityOfBrighton.Contensis.OpeningHours\OpenTimePeriod.cs
3. UniversityOfBrighton.Contensis.OpeningHours\OpenTimeChecker.cs
4. UniversityOfBrighton.Contensis.OpeningHours\OpenTimePeriodReader.cs

Usage
---

See the ExampleRazors folder Pages\Index.cshtml for an example razor view which displays an open or closed message. It's easier on the CMS because you don't need a Client ID or Shared Secret.

```cs
@using System
@using Zengenti.Contensis.Delivery
@using UniversityOfBrighton.Contensis.OpeningHours
@{
    var testKey = "0/1275/1281";
    var client = ContensisClient.Create();
    var periodList = OpenTimePeriodReader.FetchOpenTimePeriods(client, testKey, "openTimePeriod");
    var checker = new OpenTimeChecker(periodList);
    
     // Allows editer to check what ouput will be on Preview server
    var now = ( Request.QueryString["date"] != null && Request.QueryString["time"] != null) ? 
        DateTime.Parse(Request.QueryString["date"] + " " + Request.QueryString["time"]) :
        DateTime.Now;
}
<div>
    @if(checker.IsOpen(now))
    {
        <p>OPEN</p>
    }
    else
    {
        <p>CLOSED</p>
    }
</div>
```