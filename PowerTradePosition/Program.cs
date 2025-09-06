using Axpo;
using PowerTradePosition.Enums;
using PowerTradePosition.Logs;

namespace PowerTradePosition
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("PowerTradePosition is starting...");
                Console.WriteLine("Starting first default execution");
                ushort[][] hoursMapping = definehoursMapping();
                double[] volumesAggregate = new double[(int)PeriodEnum.MaxSize];
                DateTime currentDatetime = DateTime.UtcNow;
                calculateDayaheadPowerposition(volumesAggregate);
                ushort[] currentHoursMapping = getHoursMapping(hoursMapping, currentDatetime);
                List<string> lines = createArrayToPrint(currentHoursMapping, volumesAggregate, currentDatetime);
                writeCsv(lines, currentDatetime);
                Console.WriteLine("Ending first default execution");
                /* TODO: Continue developing intervals logic */
                string outputPath = determineOutputPath();
                ushort interval = determineInterval();
                Console.WriteLine("PowerTradePosition says bye!");
            }

            catch (Exception ex)
            {
                Console.WriteLine("An error has happened. Please check the logs");
                string fileName = "PowerTradePositionLog_" + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ssZ") + ".txt";

                if (WriteLogFile.WriteLog(fileName, ex.ToString()))
                {
                    Console.WriteLine("Log file written successfully");
                }

                else
                {
                    Console.WriteLine("Please contact the IT team, logs could not be written");
                }
            }
        }

        private static ushort[][] definehoursMapping()
        {
            ushort[] zero = [23, 1];
            ushort[] one = [22, 2];
            ushort[] two = [21, 3];
            ushort[] three = [20, 4];
            ushort[] four = [19, 5];
            ushort[] five = [18, 6];
            ushort[] six = [17, 7];
            ushort[] seven = [16, 8];
            ushort[] eight = [15, 9];
            ushort[] nine = [14, 10];
            ushort[] ten = [13, 11];
            ushort[] eleven = [12, 12];
            ushort[] twelve = [11, 13];
            ushort[] thirteen = [10, 14];
            ushort[] fourteen = [9, 15];
            ushort[] fifteen = [8, 16];
            ushort[] sixteen = [7, 17];
            ushort[] seventeen = [6, 18];
            ushort[] eighteen = [5, 19];
            ushort[] nineteen = [4, 20];
            ushort[] twenty = [3, 21];
            ushort[] twentyone = [2, 22];
            ushort[] twentytwo = [1, 23];
            ushort[] twentythree = [0, 24];

            ushort[][] result = [zero,
                one,
                two,
                three,
                four,
                five,
                six,
                seven,
                eight,
                nine,
                ten,
                eleven,
                twelve,
                thirteen,
                fourteen,
                fifteen,
                sixteen,
                seventeen,
                eighteen,
                nineteen,
                twenty,
                twentyone,
                twentytwo,
                twentythree
                                                ];
            return result;
        }

        private static void calculateDayaheadPowerposition(double[] volumesAggregate)
        {
            PowerService powerService = new PowerService();
            DateTime dayAhead = DateTime.UtcNow.AddDays(1);
            IEnumerable<PowerTrade> tradePositions = powerService.GetTrades(dayAhead);

            foreach (PowerTrade trade in tradePositions)
            {
                for (int i = 0; i < volumesAggregate.Length; i++)
                {
                    volumesAggregate[i] += trade.Periods[i].Volume;
                }
            }
        }

        private static ushort[] getHoursMapping(ushort[][] hoursMapping, DateTime currentDatetime)
        {
            ushort currentHour = (ushort)currentDatetime.Hour;
            ushort[] result = new ushort[24];

            switch (currentHour)
            {
                case 0: result = hoursMapping[0]; break;
                case 1: result = hoursMapping[1]; break;
                case 2: result = hoursMapping[2]; break;
                case 3: result = hoursMapping[3]; break;
                case 4: result = hoursMapping[4]; break;
                case 5: result = hoursMapping[5]; break;
                case 6: result = hoursMapping[6]; break;
                case 7: result = hoursMapping[7]; break;
                case 8: result = hoursMapping[8]; break;
                case 9: result = hoursMapping[9]; break;
                case 10: result = hoursMapping[10]; break;
                case 11: result = hoursMapping[11]; break;
                case 12: result = hoursMapping[12]; break;
                case 13: result = hoursMapping[13]; break;
                case 14: result = hoursMapping[14]; break;
                case 15: result = hoursMapping[15]; break;
                case 16: result = hoursMapping[16]; break;
                case 17: result = hoursMapping[17]; break;
                case 18: result = hoursMapping[18]; break;
                case 19: result = hoursMapping[19]; break;
                case 20: result = hoursMapping[20]; break;
                case 21: result = hoursMapping[21]; break;
                case 22: result = hoursMapping[22]; break;
                case 23: result = hoursMapping[23]; break;
            }

            return result;
        }

        private static List<string> createArrayToPrint(ushort[] currentHoursMapping, double[] volumesAggregate, DateTime currentDatetime)
        {
            List<string> lines = new List<string>();
            lines.Add("Datetime;Volume");

            ushort hoursInCurrentDay = currentHoursMapping[0];
            ushort hoursInDayAhead = currentHoursMapping[1];
            ushort currentHour = (ushort)currentDatetime.Hour;
            ushort currentDayCounter = 0;
            ushort dayAheadCounter = 0;
            
            bool flowControl = false;

            foreach (double volume in volumesAggregate)
            {
                if (currentDayCounter < hoursInCurrentDay)
                {
                    currentHour++;
                    DateTime iDatetime = new DateTime(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, currentHour, 0, 0);
                    lines.Add(iDatetime.ToString("yyyy-MM-ddTHH:mm:ssZ") + ";" + volume.ToString() + ";");
                    currentDayCounter++;
                    flowControl = true;
                }
                
                else
                {
                    flowControl = false;
                    
                    if (currentHour >= 23)
                    {
                        currentHour = 0;
                    }
                }

                if (!flowControl && dayAheadCounter < hoursInDayAhead)
                {
                    DateTime xDatetime = new DateTime(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, currentHour, 0, 0);
                    DateTime yDatetime = xDatetime.AddDays(1);
                    lines.Add(yDatetime.ToString("yyyy-MM-ddTHH:mm:ssZ") + ";" + volume.ToString() + ";");
                    currentHour++;
                    dayAheadCounter++;
                }
            }

            return lines;
        }

        private static void writeCsv(List<string> lines, DateTime currentDatetime)
        {
            string fileName = "PowerPosition";
            DateTime dateAhead = currentDatetime.AddDays(1);
            fileName += "_" + dateAhead.ToString("yyyy-MM-dd");
            fileName += "_" + currentDatetime.ToString("yyyy-MM-ddHHmm");
            fileName += ".csv";

            string docPath =
              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName)))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }

        private static string determineOutputPath()
        {
            string result = "";
            string curFile = @"C:\Projects\PowerTradePosition\PowerTradePosition\InputExamples\OutputPath.xml";
            string outputPathType = (File.Exists(curFile) ? "outputPathByXml" : "outputPathByConsole");

            switch (outputPathType)
            {
                case "outputPathByXml":
                    result = getOutputPathFromXml();
                    break;
                case "outputPathByConsole":
                    Console.WriteLine("Insert the output path:");
                    result = Console.ReadLine();
                    Console.WriteLine("output path set to: " + result);
                    break;
            }
            return result;
        }

        private static ushort determineInterval()
        {
            ushort result = 0;
            string curFile = @"C:\Projects\PowerTradePosition\PowerTradePosition\InputExamples\Interval.xml";
            string intervalType = (File.Exists(curFile) ? "intervalByXml" : "intervalByConsole");

            switch (intervalType)
            {
                case "intervalByXml":
                    result = getIntervalFromXml();
                    break;
                case "intervalByConsole":
                    Console.WriteLine("Insert the interval in minutes:");
                    result = ushort.Parse(Console.ReadLine());
                    Console.WriteLine("interval to: " + result);
                    break;
            }
            return result;
        }

        private static string getOutputPathFromXml()
        {
            throw new NotImplementedException();
        }

        private static ushort getIntervalFromXml()
        {
            throw new NotImplementedException();
        }        

    }
}
