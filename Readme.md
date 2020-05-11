# Wiki Season Retriever

- [Wiki Season Retriever](#wiki-season-retriever)
  - [Description](#description)
  - [Program Overview Usage](#program-overview-usage)
    - [Television Series Wikipedia Page Types](#television-series-wikipedia-page-types)
      - [Example:](#example)
    - [Type 1 with ONLY one season](#type-1-with-only-one-season)
    - [Entering Television Series Wikipedia Page URLs](#entering-television-series-wikipedia-page-urls)
    - [Type 1 `List of ... pages`](#type-1-list-of--pages)
    - [Type 2 `List of ... pages`](#type-2-list-of--pages)
  - [Method/Function Details](#methodfunction-details)
    - [`PrintLogo()`](#printlogo)
    - [`GetWikiURI()`](#getwikiuri)
    - [`GetWikiSubdirectory()`](#getwikisubdirectory)
    - [`GetWikiSectionJson()`](#getwikisectionjson)
    - [`CheckSingleOrMultiSeason()`](#checksingleormultiseason)
    - [`GetEpisodeListIndex()`](#getepisodelistindex)
    - [`ShowSeasonAndIndex()`](#showseasonandindex)
    - [`GetSeasonSection()`](#getseasonsection)
    - [`GetSeasonJson()`](#getseasonjson)
  - [Version Descriptions](#version-descriptions)

## Description
This is a prototype of the Wiki Season Retriever, which combines the concepts and skills used in the *WikiTemplateParser* and *WikiRestRequest* program and planned for a more refined usage as a console application.  Additional features will be added to this, such as inserting and retrieving data to and from a database.

As a reminder, this is a prototype to practice the following skills.  Eventually this project will be rebuilt as a web application, further practicing more skills.

The following skills demonstrated in the *Wiki Season Retriever*:
* RESTful API Requests,
* JSON parsing,
* Parsing and Scraping of retrieved data (in this case: WikiText),
* Regular Expressions,
* Application planning and design, and
* Clean code.

Note: This will be added to as developement continues.  You can also follow the blog entries which follows my learning and development of this program and will be linked here soon.

## Program Overview Usage

### Television Series Wikipedia Page Types
There are several variations and formats of television series Wikipedia pages.  First, the "`List of ...`" television series must be found.  These pages list out all seasons (and episodes) in this page.

These types of pages, henceforth as `List of ... pages`, can be divided into two types:

**Type 1** - All seasons are listed.

**Type 2** - All seasons listed but have season pages.

#### Example:
Type 1:
```
https://en.wikipedia.org/wiki/List_of_Star_Wars:_The_Clone_Wars_episodes
```
Type 2:
```
https://en.wikipedia.org/wiki/List_of_My_Hero_Academia_episodes
```
* As you can see, each season will have their own wikipedia page.
  * E.g. `Main article: My Hero Academia (season 1)`

<br>
<br>

### Type 1 with ONLY one season
You will first be prompted with this message:

```
Is there ONLY ONE season in the series page? [Y/N]
```
If the series page only has one season, you must enter `Y` for the program to work.

These are series with no individual season wikipedia page.

<br><br>

### Entering Television Series Wikipedia Page URLs
These URLs (examples mentioned above) are what must be entered into the program at the prompt:

```
Enter Wikipedia URL:
```
The program will also inform you whether it is a **Type 1** or **Type 2** `List of ... pages`.

<br><br>

### Type 1 `List of ... pages`
When the program detects a Type 1 `List of ... page` it will then present you with index numbers and the associated season.

Only individual seasons can be parsed and scraped at this time.  You will be prompted with this message to enter the **index number** (not the season number):

```
Enter index of chosen season: 
```

It will then output the season in a WikiText format.

<br><br>

### Type 2 `List of ... pages`
When the program detects a Type 2 `List of ... page`, it will present the season in a WikiText format.

## Method/Function Details

I try to implement a DRY and Clean Code design when creating the methods for this program.  These will change over time as I refine the program as I see fit.

Method list:
* `PrintLogo()`
* `GetWikiURI()`
* `GetWikiSubdirectory()`
* `GetWikiSectionJson()`
* `CheckSingleOrMultiSeason()`
* `GetEpisodeListIndex()`
* `ShowSeasonAndIndex()`
* `GetSeasonSection()`
* `GetSeasonJson()`

### `PrintLogo()`
Prints the golden retriever logo in ASCII art.

### `GetWikiURI()`
Parses the URL from user input, using `GetWikiSubdirectory()`, and returns a string of the Wikipedia API URI:
```csharp
string wikiUriFormat = "https://en.wikipedia.org/w/api.php?action=parse&format=json&prop=sections&page="
```

### `GetWikiSubdirectory()`
Slices the wikipedia URL and returns the subdirectory, essential the name of the Wikipedia page, as a string.


### `GetWikiSectionJson()`
Uses the URI provided by `GetWikiURI()` to access Wikipedia's RESTful API services and retrieves and deserializes the JSON string, which are the sections of the Wikipedia page.

### `CheckSingleOrMultiSeason()`
Returns a boolean after checking if the JSON string from `GetWikiSectionJson()` contains either "Episode List" or "Episodes" (hardcoded. Still looking for an alternative method).

This will dictate whether the `Main()` will process the JSON as a single season (Type 2), multi-season (Type 1), or one season in a multi-season page (Type 1)

### `GetEpisodeListIndex()`
Looks for the section index of the section named "Episode List" in the retrieved JSON.  Returns the index number as an integer.

Part of the "Single-season page methods" in the Program.cs.

### `ShowSeasonAndIndex()`
Uses a regex pattern to find season sections in a Type 2 `List of ... page`.  

### `GetSeasonSection()`

### `GetSeasonJson()`



## Version Descriptions