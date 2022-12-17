using UnityEngine;

[System.Serializable]
public class TranslatedContent
{
    public int LanguageId;
    public string LanguageName;
    public Topic[] Topics;
}

[System.Serializable]
public class Topic
{
    public string Name;
    public Media[] Media;
}

[System.Serializable]
public class Media
{
    public string Name;
    public string FilePath;
    public Photo[] Photos;
}

[System.Serializable]
public class Photo
{
    public string Path;
    public string Name;
}

[System.Serializable]
public class JsonClassList
{
    public TranslatedContent[] TranslatedContents;
}