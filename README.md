# DataConveyer_LoadSparseData

DataConveyer_LoadSparseData is a console application to demonstrate how Data Conveyer can be
used to generate SQL scripts to load data into a database.

It can be thought of as an "intelligent BCP script" ([BCP](https://docs.microsoft.com/en-us/sql/tools/bcp-utility?view=sql-server-2017) = Bulk Copy Program) that evaluates
large input text file "on the fly" and decides what data needs to be loaded. Custom logic is
used to filter out unwanted records, remove unneded data elements, perform calculations, such
as data decryption, etc.

Such data loader (pre-scrubber) can be helpful when large datasets get loaded into a database
for ad-hoc analyses. When it is known up-front that many records/elements are irrelevant to
such analyses, it may be sensible to only load those items that are relevant.

This somewhat contrived example assumes an input data to contain a list of people with many
extraneous fields and many records missing data (and as such being irrelevant). Like so:

```
Id      Key            Code                              Filler1                                  FName       LName               DOB       Filler2                                         Hash                                                            Filler3         Street                       City                   StateZip  Phone         Email                                 Guid                                Filler4                                             EncSsn
       1xR7So1sVc      FR941490773748V7OWDUDSRLR35                                                Aloin       Shakspeare            8/5/1995                                                f66b20f26829013c4dacd0de98eb8db2876ad633971915d03b1c088a43d7a00a                73 American Avenue           Honolulu               HI   96805(808) 663-4829ashakspearerr@webs.com                4f081ef2-340d-4a95-92cb-c1c8d9e3ab7f                                                    hfd^jj^hdeh
       2               SM92P9521830812ZJY4UYCJJTDJ                                                                                                                                          7befc77278997afd39fc3fb6a1d2808cce48c5ea71c172ac2d2e29f42eeaa9b9                                                                                                                                  ce1525ab-b9f0-4993-a850-d29a687cac5f
```

Upon execution, a new file will be created with SQL script that loads only relevant data into
the database. Like so:

```sql
CREATE TABLE MyPeople (ID char(12), LastName char(20), FirstName char(12), SSN char(11))
GO
INSERT INTO MyPeople VALUES('xR7So1sVc', 'Shakspeare', 'Aloin', '753-99-7347')
```

## Installation

* Fork this repository and clone it onto your local machine, or

* Download this repository onto your local machine.

## Usage

1. Open DataConveyer_LoadSparseData application in Visual Studio.

2. Build and run the application, e.g. hit F5

    - a console window with directions will show up.

3. Copy an input file (SparseData.txt extracted from SparseData.zip) into the ...Data\In folder

    - the file will get processed as reported in the console window.

4. Review the contents of the output file placed in the ...Data\Out folder.

5. (optional) Repeat steps 3-4 for other additional input file(s).

6. To exit application, hit Enter key into the console window.

**Note:** If you  experience *"InitializationError"* after copying the input file in step 3, it
most likely happens because the file is still being copied (and hence locked) when Data&nbsp;Conveyer
attempts to start processing. To circumvent this condition, you can try moving the file (from a location
on the same partition) instead of copying it. It this doesn't help, then consider refactoring
the code like in the [DataConveyer_FilterLargeCsvData](https://github.com/mavidian/DataConveyer_FilterLargeCsvData)
repository, where the process start is deferred until explicit user action.

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

[Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)

## Copyright

```
Copyright © 2019-2020 Mavidian Technologies Limited Liability Company. All Rights Reserved.
```
