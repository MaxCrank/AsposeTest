# Aspose format conversion library

The library is intended to process significant data items between various format layouts. Initially it's made for XML 
(schema is [here](AsposeFormatConverter/FormatProcessors/XML/XmlFormatSchema.xsd)) and custom binary file (see 
[BinaryFormatSerializationData](AsposeFormatConverter/FormatProcessors/Binary/BinaryFormatSerializationData.cs)).

## Architecture

In general, the idea was to maximally hide the implementation from the user and work heavily with abstractions to ease the 
development process and provide extensibility.

All format processors, which implement features for data manipulation, are populated by main
[CommonFormatConverter](AsposeFormatConverter/Base/CommonFormatConverter.cs) class using `CreateFormatProcessor` method. Also, 
it has higl-level `Convert` method to transofrm data between formats in case user doesn't need any intermidiate manipulation. 
It implements [ICommonFormatConverter](AsposeFormatConverter/Base/Interfaces/ICommonFormatConverter.cs) interface, which is
hidden from the user and used for development process only.

Each format processor is implementing [IFormatProcessor](AsposeFormatConverter/Base/Interfaces/IFormatProcessor.cs) interface, 
through wich users are interacting with an instance: manipulating data and processing files. Data items are implementing
[IFormatDataItem](AsposeFormatConverter/Base/Interfaces/IFormatDataItem.cs) interface and can be accessed by indexer,
`IEnumerable` or `Data` collection; collection is processed with `SetData`, `AddNewDataItem`, `AddItem`,`RemoveDataItem` and 
`ClearData` methods. Base class that contains general features for a typical format processor is 
[FormatProcessorBase](AsposeFormatConverter/Base/Abstracts/FormatProcessorBase.cs).

Separate data item manipulation is provided through [IFormatDataItem](AsposeFormatConverter/Base/Interfaces/IFormatDataItem.cs) 
interface, which gives ability for the user to set date, brand name and price. 
[FormatDataItem](AsposeFormatConverter/Base/FormatDataItem.cs) class implements 
[IFormatDataItem](AsposeFormatConverter/Base/Interfaces/IFormatDataItem.cs) and is used in 
[FormatProcessorBase](AsposeFormatConverter/Base/Abstracts/FormatProcessorBase.cs).

### Pros

1. Extensibility. Each new format processor can be easily added by inheriting from the 
[FormatProcessorBase](AsposeFormatConverter/Base/Abstracts/FormatProcessorBase.cs), which already has most of needed
functionality - you only need to implement format-specific features.
2. Abstractions are properly separated from the implementation.
3. Encapsulation. Previous pro gives developer an abilty to give public access only to the [CommonFormatConverter]
(AsposeFormatConverter/Base/CommonFormatConverter.cs), leaving everything else for interfaces, which is good for class 
libraries.
4. Unit testing. It covers almost 90% of code, and is already implemented to cover all basic format processor features 
through iteration of valid formats in test cases, so adding another format processor won't significantly decrease code coverage.

### Cons

1. Each format processor implementation is in the same project as the others, including base classes and abstractions, which may 
lead to a sort of a mess if there are dozens of formats.

### Future development

1. Separate base classes and abstractions from format processors with different projects for more convinient development
process, where each format processor has it's own project. For this, initialization process of 
2. Find a way to validate and parse binary files with some sort of schema (since they are widely used and heavily customized). 
[BeeSchema](https://github.com/Epidal/BeeSchema) has bugs right now, but is still in development process, so it may be used in 
the future.
3. Implement features that allow library to work not only with file paths, but streams etc. for reading and writing data on the
user side (though in the end, writing is already performed to a stream now).
