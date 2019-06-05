// Copyright © 2019 Mavidian Technologies Limited Liability Company. All Rights Reserved.

using Mavidian.DataConveyer.Common;
using Mavidian.DataConveyer.Entities.KeyVal;
using Mavidian.DataConveyer.Orchestrators;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataConveyer_LoadSparseData
{
   /// <summary>
   /// Represents Data Conveyer functionality specific to loading sparse data into a database.
   /// </summary>
   internal class FileProcessor
   {
      private readonly IOrchestrator Orchestrator;

      internal FileProcessor(string inFile, string outLocation)
      {
         var config = new OrchestratorConfig()
         {
            InputDataKind = KindOfTextData.Flat,
            InputFileName = inFile,
            HeadersInFirstInputRow = true,
            TrimInputValues = true,
            InputFields = "|8,|15,|34,|41,|12,|20,|10,|48,|64,|16,|29,|23,|5,|5,|14,|38,|36,|52,|11",
            AllowTransformToAlterFields = true,
            ConcurrencyLevel = 4,
            TransformerType = TransformerType.Universal,
            UniversalTransformer = FilterRecsAndExtractFields,
            OutputDataKind = KindOfTextData.Arbitrary,
            ArbitraryOutputDefs = new string[] { "INSERT INTO MyPeople VALUES('{Key}', ", "'{LName}', ", "'{FName}', ", "'{SSN}')" },
            LeaderContents = "CREATE TABLE MyPeople (ID char(12), LastName char(20), FirstName char(12), SSN char(11))\r\nGO",
            OutputFileName = outLocation + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(inFile) + ".sql"
         };

         Orchestrator = OrchestratorCreator.GetEtlOrchestrator(config);
      }

      /// <summary>
      /// Execute Data Conveyer process.
      /// </summary>
      /// <returns>Task containing the process results.</returns>
      internal async Task<ProcessResult> ProcessFileAsync()
      {
         var result = await Orchestrator.ExecuteAsync();
         Orchestrator.Dispose();

         return result;
      }


      /// <summary>
      /// Universal transformer to translate an input cluster into a set of (0 or 1) output clusters.
      /// </summary>
      /// <param name="inClstr"></param>
      /// <returns>A single resulting cluster to be sent to output, or empty if cluster needs to be filtered out.</returns>
      private IEnumerable<ICluster> FilterRecsAndExtractFields(ICluster inClstr)
      {
         var inRec = inClstr[0]; //the only record in cluster

         if (string.IsNullOrWhiteSpace((string)inRec["Key"])) return Enumerable.Empty<ICluster>();

         var retVal = inClstr.GetEmptyClone();
         var outRec = inRec.GetEmptyClone();

         outRec.AddItem("Key", inRec["Key"]);
         outRec.AddItem("FName", EscapeQuotes((string)inRec["FName"]));
         outRec.AddItem("LName", EscapeQuotes((string)inRec["LName"]));
         outRec.AddItem("SSN", DecryptSsn((string)inRec["EncSsn"]));

         retVal.AddRecord(outRec);

         return Enumerable.Repeat(retVal, 1);
      }

      private string EscapeQuotes(string name)
      {
         //Make sure names, such as O'Neil are properly formatted in SQL script
         return name.Replace("'", "''");
      }

      private string DecryptSsn(string encSsn)
      {
         //A simulated algorithm to decrypt sensitive data that was encrypted on intake
         return new string(encSsn.Select(c => (char)((int)c - 49)).ToArray());
      }

   }
}
