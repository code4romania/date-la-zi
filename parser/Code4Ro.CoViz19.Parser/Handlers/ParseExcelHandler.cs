﻿using Code4Ro.CoViz19.Parser.Commands;
using Code4Ro.CoViz19.Parser.Models;
using CSharpFunctionalExtensions;
using ExcelDataReader;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Code4Ro.CoViz19.Parser.Handlers
{
    public class ParseExcelHandler : IRequestHandler<ParseExcelCommand, Result<ParsedDataModel>>
    {
        public async Task<Result<ParsedDataModel>> Handle(ParseExcelCommand request, CancellationToken cancellationToken)
        {
            DataSet result;

            await Task.FromResult(true);

            try
            {
                using (var stream = request.File.OpenReadStream())
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        result = reader.AsDataSet();
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<ParsedDataModel>("Upload Failed, could not read the file. " + ex.Message);
            }

            if (result == null || result.Tables == null)
            {
                return Result.Failure<ParsedDataModel>("Upload Failed, could not read the file.");
            }



            var liveData = ParseLiveData(result.Tables[0]);
            ///  var patiens = ParsePatiens(result.Tables[1]);
            //  var countiesData = ParseCountiesData(result.Tables[2]);
            var parsedData = new ParsedDataModel()
            {
                LiveUpdateData = liveData.IsSuccess ? liveData.Value : null
            };

            return Result.Ok(parsedData);
        }

        private object ParsePatiens(DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        private object ParseCountiesData(DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        private Result<LiveUpdateData[]> ParseLiveData(DataTable liveData)
        {
            if (liveData == null)
            {
                return Result.Failure<LiveUpdateData[]>("Upload Failed, Live data table is empty");
            }

            var colCount = liveData.Columns.Count;

            if (colCount < 10)
            {
                return Result.Failure<LiveUpdateData[]>("Upload Failed, live data col count");
            }
            var rowCount = liveData.Rows.Count;
            if (rowCount < 1)
            {
                return Result.Failure<LiveUpdateData[]>("Upload Failed, live data row count");
            }

            var parsedLiveData = new List<LiveUpdateData>();
            string gruppedRowDate = string.Empty;
            for (int index = 1; index < liveData.Rows.Count; index++)
            {
                DataRow row = liveData.Rows[index];
                if (!DBNull.Value.Equals(row[0]))
                {
                    gruppedRowDate = ToSafeText(row[0]);
                }
                var parsedRow = ParseLiveUpsdateDataRow(row, gruppedRowDate);
                parsedLiveData.Add(parsedRow);
            }


            var latestDataPerDay = parsedLiveData
                   .Select(x => new { key = x.Timestamp.ToShortDateString(), data = x })
                   .GroupBy(x => x.key, x => x.data, (key, rows) => rows.OrderByDescending(x => x.Timestamp).FirstOrDefault());

            return Result.Ok(latestDataPerDay.ToArray());
        }

        private LiveUpdateData ParseLiveUpsdateDataRow(DataRow row, string gruppedRowDate)
        {
            var dayMonth = gruppedRowDate.Trim().Split(".");
            int? hour = ParseInt(row[1]);
            var day = int.Parse(dayMonth[0]);
            var month = int.Parse(dayMonth[1]);
            var timestamp = new DateTime(2020, month, day, hour ?? 0, 0, 0);

            return new LiveUpdateData()
            {
                Timestamp = timestamp,
                NumberDiagnosed = ParseInt(row[2]),
                NumberCured = ParseInt(row[3]),
                NumberQuarantined = ParseInt(row[4]),
                NumberMonitoredAtHome = ParseInt(row[5]),
                EmergencyCalls = ParseInt(row[6]),
                HotLineCalls = ParseInt(row[7]),
                NumberCriminalCases = ParseInt(row[8]),
                ProbesAnalyzed = ParseInt(row[9]),
                ProbesInAnalysis = ParseInt(row[10])
            };

        }


        private int? ParseInt(object value)
        {
            if (!DBNull.Value.Equals(value))
            {
                int returnValue = 0;

                if (value is decimal)
                {
                    returnValue = (int)value;
                }

                if (value is double)
                {
                    returnValue = Convert.ToInt32(value);
                }

                if (value is float)
                {
                    returnValue = Convert.ToInt32(value);
                }

                if (value is int)
                {
                    returnValue = Convert.ToInt32(value);
                }

                if (value is string)
                {
                    if(int.TryParse(ToSafeText(value), out returnValue) == false)
                    {
                        return null;
                    }
                }

                return returnValue;
            }

            return null;
        }

        private static string ToSafeText(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var text = value.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return text;

        }
    }
}