# AlphabetizeIt for Rider and ReSharper

[![Rider](https://img.shields.io/jetbrains/plugin/v/ReSharperPlugin.AlphabetizeIt.svg?label=Rider&colorB=0A7BBB&style=for-the-badge&logo=rider)](https://plugins.jetbrains.com/plugin/ReSharperPlugin.AlphabetizeIt)
[![ReSharper](https://img.shields.io/jetbrains/plugin/v/ReSharperPlugin.AlphabetizeIt.svg?label=ReSharper&colorB=0A7BBB&style=for-the-badge&logo=resharper)](https://plugins.jetbrains.com/plugin/ReSharperPlugin.AlphabetizeIt)

AlphabetizeIt! is a plugin for Resharper and Rider that will sort the properties of a class or object initializer in alphabetical order.

A seemingly simple premise, you should be able to sort the properties of your model classes and your object initializers alphabetically! This is very helpful in larger simple mapping methods, larger object initializers, and when working in larger model and DTO class files. It almost feels like this should be a built-in feature of the IDE!

Let's say you have model file with seemingly randomly-inserted properties:
```csharp
public class EntryModel
{
    [Key]
    public int EntryId {get; set; }
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    [MaxLength(200, ErrorMessage = "Name cannot be more than 200 characters.")]
    public string Name { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public Guid? AssignedUser { get; set; }
    public int? ParentId { get; set; }
    public string ClosedDescription { get; set; }
    public string AttachmentUri { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime? LastEdited { get; set; }
    public Guid? LastEditedBy { get; set; }
    public DateTime? ClosedDate { get; set; }
    public Guid? ClosedBy { get; set; }
    public DateTime? TargetCloseDate { get; set; }
    public string Source { get; set; }
    public Guid? CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
```

Pretend its twice this big. You need to read through this to verify it matches something outside the IDE. A log, a json file, or maybe you're just a little OCD like me.
Either way, it would be a lot easier to verify or read or understand if it were in alphabetical order! This plugin gives you this menu option:

![screenshot3.png](https://github.com/ZXeno/Resharper-Rider.AlphabetizeIt/blob/main/Content/screenshot3.png)

This will sort the properties of the class for you!<br/>
Behold!

![screenshot4.png](https://github.com/ZXeno/Resharper-Rider.AlphabetizeIt/blob/main/Content/screenshot4.png)

As simple as that. This also works for Object Initializers, too!

![screenshot5.png](https://github.com/ZXeno/Resharper-Rider.AlphabetizeIt/blob/main/Content/screenshot5.png)