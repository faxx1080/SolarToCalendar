/*
 * All Code Copyright Frank Migliorino © 2013. All Rights Reserved
 */ 


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolarToCalendar
{

    public partial class MainForm : Form
    {
        string initialDirections = "";
        string initialButtonText = "";
        public MainForm()
        {
            InitializeComponent();
            initialDirections = textBox1.Text;
            initialButtonText = button1.Text;
        }

        System.IO.StreamWriter fout;

        private int copyType;
        //0=normal; 1=shopping cart; 2=undefined

        private void convertClipboardType2(string s) {

            string rawInput = s;
            List<sbClassString> classListStr = new List<sbClassString>();
            List<string> tempSplitInput = new List<string>(rawInput.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));


            string[] splitInput = tempSplitInput.ToArray();
            // string[] splitInput = rawInput.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            string currentClassName = splitInput[i++];

            while (i < splitInput.Length) {

                sbClassString thisClass = new sbClassString();
                thisClass.name = currentClassName;
                //thisClass.number = (splitInput[i++]);
                //thisClass.section = splitInput[i++];
                //thisClass.component = splitInput[i++];
                thisClass.dateTime = splitInput[i++];
                // SPECIAL CASE - DateTime will not be here if online course
                //if (thisClass.section.ToLower().Contains("online") || thisClass.component.ToLower().Contains("online")) {
                //    i--;
                //    onlineCourse = true;
                //}
                thisClass.location = splitInput[i++];
                //while (true) {
                //    thisClass.instructors += splitInput[i++];
                //    if (char.IsDigit(splitInput[i].ElementAt(0))) { break; } //no more inst's
                //}

                //Make a range starting next monday.
                DateTime current = DateTime.Now;
                int daysToSkipAhead = (8 - (int)current.DayOfWeek) % 7;
                DateTime stDate = current.AddDays(daysToSkipAhead);
                DateTime endDate = stDate.AddDays(6);

                thisClass.startEndDate = stDate.ToShortDateString() + " - " + endDate.ToShortDateString();
                classListStr.Add(thisClass);

                if (i >= splitInput.Length) { break; }
                if (char.IsWhiteSpace(splitInput[i].ElementAt(0))) {
                    // another component, same name.
                    i++;
                    continue;
                } else {
                    {
                        // case 1: XXX #:
                        // case 2: XXX ##:
                        int x = splitInput[i].IndexOf(' ', 0) + 1;
                        if (Char.IsDigit((char)splitInput[i].ElementAt(x))) {
                            if (splitInput[i].ElementAt(x + 1) == ':' ||
                                (Char.IsDigit(splitInput[i].ElementAt(x + 1)) &&
                                splitInput[i].ElementAt(x + 2) == ':')) { continue; }
                        }
                    }
                    currentClassName = splitInput[i++];
                }
            }
            i = 0;
            // All Done Parsing!

            SaveFileDialog saveOut = new SaveFileDialog();
            saveOut.AddExtension = true;
            saveOut.DefaultExt = ".ics";
            saveOut.Filter = "iCalendar File|*.ics";
            saveOut.Title = "Save ICS File";
            DialogResult saveDiag = saveOut.ShowDialog();
            fout = null;

            if (saveDiag == System.Windows.Forms.DialogResult.OK) {
                fout = new System.IO.StreamWriter(saveOut.FileName);
            } else {
                return;
            }

            fout.WriteLine(@"BEGIN:VCALENDAR
VERSION:2.0
METHOD:PUBLISH");
            fout.WriteLine("X-WR-CALNAME:" + System.IO.Path.GetFileNameWithoutExtension(saveOut.FileName));
            fout.WriteLine(@"BEGIN:VTIMEZONE
TZID:Eastern Standard Time
BEGIN:STANDARD
DTSTART:16011104T020000
RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
END:STANDARD
BEGIN:DAYLIGHT
DTSTART:16010311T020000
RRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
END:DAYLIGHT
END:VTIMEZONE");

            Random rng = new Random();

            foreach (sbClassString xClass in classListStr) {
                fout.Write("BEGIN:VEVENT\nCLASS:PUBLIC\nPRIORITY:5\nSEQUENCE:0\n" +
                    "STATUS:CONFIRMED\nTRANSP:OPAQUE\nX-MICROSOFT-CDO-BUSYSTATUS:BUSY\nUID:");
                fout.WriteLine(rng.Next(1000000000, int.MaxValue).ToString() + rng.Next(1000000000, int.MaxValue).ToString());

                // Getting the time of the class

                string tempDTS = xClass.startEndDate.Substring(0, xClass.startEndDate.IndexOf(" "));
                string tempDTE = xClass.startEndDate.Substring(xClass.startEndDate.IndexOf("-") + 2);
                string tempDaysTimes = xClass.dateTime;
                string tempTimes = tempDaysTimes.Substring(tempDaysTimes.IndexOf(" ") + 1);
                // Assumptions made on copied schedule:
                // 1) Date Start is Monday
                // so to get the dates for classes, take the first day offered, and use
                // that to offset from that starting day.
                int offset = 0;
                switch (tempDaysTimes.Substring(0, 2)) {
                    case "Mo":
                        offset = 0;
                        break;
                    case "Tu":
                        offset = 1;
                        break;
                    case "We":
                        offset = 2;
                        break;
                    case "Th":
                        offset = 3;
                        break;
                    case "Fr":
                        offset = 4;
                        break;
                    case "Sa":
                        offset = 5;
                        break;
                    case "Su":
                        offset = 6;
                        break;
                }
                int hour = int.Parse(tempTimes.Substring(0, tempTimes.IndexOf(":") - 0));
                int min = int.Parse(tempTimes.Substring(tempTimes.IndexOf(':') + 1, 2));
                if (tempTimes.ElementAt(tempTimes.IndexOf("-") - 3) == 'P' && hour != 12) { hour += 12; }
                TimeSpan tStart = new TimeSpan(hour, min, 0);

                tempTimes = tempTimes.Substring(tempTimes.IndexOf("-") + 2);

                hour = int.Parse(tempTimes.Substring(0, tempTimes.IndexOf(":") - 0));
                min = int.Parse(tempTimes.Substring(tempTimes.IndexOf(':') + 1, 2));
                if (tempTimes.ElementAt(tempTimes.IndexOf(":") + 3) == 'P' && hour != 12) { hour += 12; }
                TimeSpan tEnd = new TimeSpan(hour, min, 0);


                DateTime classStarts = DateTime.Parse(tempDTS).AddDays(offset).Add(tStart);
                DateTime classEnds = DateTime.Parse(tempDTS).AddDays(offset).Add(tEnd);
                DateTime classReallyEnds = DateTime.Parse(tempDTE);
                // end getting time

                fout.Write("DTSTART;TZID=\"Eastern Standard Time\":");
                fout.WriteLine(String.Format("{0:yyyyMMdd'T'HHmm'00'}", classStarts));
                fout.Write("DTEND;TZID=\"Eastern Standard Time\":");
                fout.WriteLine(String.Format("{0:yyyyMMdd'T'HHmm'00'}", classEnds));
                string daysRepeated = tempDaysTimes.Substring(0, tempDaysTimes.IndexOf(" ") + 1).ToUpper();
                {
                    List<char> daysRep = new List<char>(daysRepeated.ToCharArray());
                    daysRep.TrimExcess();
                    int j = 0;
                    while (j < daysRep.Count) {
                        j += 2;
                        if (j < daysRep.Count - 1) {
                            daysRep.Insert(j, ',');
                            j++;
                        }
                    }
                    daysRep.RemoveAt(daysRep.Count - 1);
                    daysRepeated = new string(daysRep.ToArray());
                }

                fout.WriteLine("RRULE:FREQ=WEEKLY;UNTIL=" + classReallyEnds.ToString("yyyyMMdd") + "T223000;BYDAY=" + daysRepeated);
                fout.WriteLine("DESCRIPTION:--");
                fout.WriteLine("LOCATION:" + xClass.location);
                fout.WriteLine("SUMMARY:" + xClass.fullName());
                fout.WriteLine("END:VEVENT");

            }
            fout.WriteLine("END:VCALENDAR");

            fout.Flush();
            fout.Close();
        }

        private void convertClipboard(string s) {
            /* Text file format:
             * Name
             * count tabs + 1 lines
             * "Class\tNbr"
             * ClassNumber
             * Section
             * Component
             * Time: MoTuWeThFr ##:##XM - ##:##XM
             * Location
             * Inst1
             * Inst2
             * ...
             * ##/##/#### - ##/##/#### (Date Active)
             * ****IF # then REPEAT @ ClassNbr (Same Title)
             * ELSE GOTO Name
             */

            string rawInput = s;
            List<sbClassString> classListStr = new List<sbClassString>();
            List<string> tempSplitInput = new List<string>(rawInput.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
            for (int x = 0; x < tempSplitInput.Count; x++) {
                if (tempSplitInput.ElementAt(x) == "\t") {
                    tempSplitInput.RemoveAt(x);
                    x--;
                }
            }
            string[] splitInput = tempSplitInput.ToArray();
            // string[] splitInput = rawInput.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            string currentClassName = splitInput[i++];
            // credit to tchrist on StackOverflow for this:
            string countTabs = splitInput[i++];
            string countTabsSmaller = countTabs.Replace("\t", "");
            int numTabs = countTabs.Length - countTabsSmaller.Length + 2;
            i += numTabs;
            // end credit
            bool onlineCourse = false;
            while (i < splitInput.Length) {

                sbClassString thisClass = new sbClassString();
                thisClass.name = currentClassName;
                thisClass.number = (splitInput[i++]);
                thisClass.section = splitInput[i++];
                thisClass.component = splitInput[i++];
                thisClass.dateTime = splitInput[i++];
                // SPECIAL CASE - DateTime will not be here if online course
                if (thisClass.section.ToLower().Contains("online") || thisClass.component.ToLower().Contains("online")) {
                    i--;
                    onlineCourse = true;
                }
                thisClass.location = splitInput[i++];
                while (true) {
                    thisClass.instructors += splitInput[i++];
                    if (char.IsDigit(splitInput[i].ElementAt(0))) {break;} //no more inst's
                }
                thisClass.startEndDate = splitInput[i++];
                if (!onlineCourse) { classListStr.Add(thisClass); }
                onlineCourse = false;
                if (i >= splitInput.Length) { break; }
                if (char.IsDigit(splitInput[i].ElementAt(0))) {
                    // another component, same name.
                    continue;
                } else {
                    currentClassName = splitInput[i++];
                    // credit to tchrist on StackOverflow for this:
                    countTabs = splitInput[i++];
                    countTabsSmaller = countTabs.Replace("\t", "");
                    numTabs = countTabs.Length - countTabsSmaller.Length + 2;
                    i += numTabs;
                    // end credit
                }
            }
            i = 0;
            // All Done Parsing!

            SaveFileDialog saveOut = new SaveFileDialog();
            saveOut.AddExtension = true;
            saveOut.DefaultExt = ".ics";
            saveOut.Filter = "iCalendar File|*.ics";
            saveOut.Title = "Save ICS File";
            DialogResult saveDiag = saveOut.ShowDialog();
            fout = null;

            if (saveDiag == System.Windows.Forms.DialogResult.OK) {
                fout = new System.IO.StreamWriter(saveOut.FileName);
            } else {
                return;
            }
            
            fout.WriteLine(@"BEGIN:VCALENDAR
VERSION:2.0
METHOD:PUBLISH");
            fout.WriteLine("X-WR-CALNAME:" + System.IO.Path.GetFileNameWithoutExtension(saveOut.FileName));
            fout.WriteLine(@"BEGIN:VTIMEZONE
TZID:Eastern Standard Time
BEGIN:STANDARD
DTSTART:16011104T020000
RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
END:STANDARD
BEGIN:DAYLIGHT
DTSTART:16010311T020000
RRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
END:DAYLIGHT
END:VTIMEZONE");

            Random rng = new Random();

            foreach (sbClassString xClass in classListStr) {
                fout.Write("BEGIN:VEVENT\nCLASS:PUBLIC\nPRIORITY:5\nSEQUENCE:0\n" +
                    "STATUS:CONFIRMED\nTRANSP:OPAQUE\nX-MICROSOFT-CDO-BUSYSTATUS:BUSY\nUID:");
                fout.WriteLine(rng.Next(1000000000, int.MaxValue).ToString() + rng.Next(1000000000, int.MaxValue).ToString());
               
                // Getting the time of the class

                string tempDTS = xClass.startEndDate.Substring(0, xClass.startEndDate.IndexOf(" "));
                string tempDTE = xClass.startEndDate.Substring(xClass.startEndDate.IndexOf("-") + 2);
                string tempDaysTimes = xClass.dateTime;
                string tempTimes = tempDaysTimes.Substring(tempDaysTimes.IndexOf(" ") + 1);
                // Assumptions made on copied schedule:
                // 1) Date Start is Monday
                // so to get the dates for classes, take the first day offered, and use
                // that to offset from that starting day.
                int offset = 0;
                switch (tempDaysTimes.Substring(0, 2)) {
                    case "Mo":
                        offset = 0;
                        break;
                    case "Tu":
                        offset = 1;
                        break;
                    case "We":
                        offset = 2;
                        break;
                    case "Th":
                        offset = 3;
                        break;
                    case "Fr":
                        offset = 4;
                        break;
                    case "Sa":
                        offset = 5;
                        break;
                    case "Su":
                        offset = 6;
                        break;
                }
                int hour = int.Parse(tempTimes.Substring(0, tempTimes.IndexOf(":") - 0));
                int min = int.Parse(tempTimes.Substring(tempTimes.IndexOf(':') + 1, 2));
                if (tempTimes.ElementAt(tempTimes.IndexOf("-") - 3) == 'P' && hour != 12) { hour += 12; }
                TimeSpan tStart = new TimeSpan(hour, min, 0);

                tempTimes = tempTimes.Substring(tempTimes.IndexOf("-") + 2);

                hour = int.Parse(tempTimes.Substring(0, tempTimes.IndexOf(":") - 0));
                min = int.Parse(tempTimes.Substring(tempTimes.IndexOf(':') + 1, 2));
                if (tempTimes.ElementAt(tempTimes.IndexOf(":") + 3) == 'P' && hour != 12) { hour += 12; }
                TimeSpan tEnd = new TimeSpan(hour, min, 0);


                DateTime classStarts = DateTime.Parse(tempDTS).AddDays(offset).Add(tStart);
                DateTime classEnds = DateTime.Parse(tempDTS).AddDays(offset).Add(tEnd);
                DateTime classReallyEnds = DateTime.Parse(tempDTE);
                // end getting time

                fout.Write("DTSTART;TZID=\"Eastern Standard Time\":");
                fout.WriteLine(String.Format("{0:yyyyMMdd'T'HHmm'00'}", classStarts));
                fout.Write("DTEND;TZID=\"Eastern Standard Time\":");
                fout.WriteLine(String.Format("{0:yyyyMMdd'T'HHmm'00'}", classEnds));
                string daysRepeated = tempDaysTimes.Substring(0, tempDaysTimes.IndexOf(" ") + 1).ToUpper();
                {
                    List<char> daysRep = new List<char>(daysRepeated.ToCharArray());
                    daysRep.TrimExcess();
                    int j = 0;
                    while (j < daysRep.Count) {
                        j += 2;
                        if (j < daysRep.Count - 1) {
                            daysRep.Insert(j, ',');
                            j++;
                        }
                    }
                    daysRep.RemoveAt(daysRep.Count - 1);
                    daysRepeated = new string(daysRep.ToArray());
                }

                fout.WriteLine("RRULE:FREQ=WEEKLY;UNTIL=" + classReallyEnds.ToString("yyyyMMdd") + "T223000;BYDAY=" + daysRepeated);
                fout.Write("DESCRIPTION:");
                fout.WriteLine(String.Format("{0}\\nClass Number: {1}\\nClass Section: {2}\\nComponent: {3}\\nDateTime: {4}" +
                    "\\nLocation: {5}\\nInstructors: {6}\\nStart and End Date: {7}",
                    xClass.fullName(), xClass.number, xClass.section,
                        xClass.component, xClass.dateTime, xClass.location, xClass.instructors, xClass.startEndDate
                    ));
                fout.WriteLine("LOCATION:" + xClass.location);
                fout.WriteLine("SUMMARY:" + xClass.fullName());
                fout.WriteLine("END:VEVENT");

            }
            fout.WriteLine("END:VCALENDAR");

            fout.Flush();
            fout.Close();
            if (MessageBox.Show("Done! Would you like to know\nhow to import " +
                "that file to\nGoogle Calendar?","Done" , MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes) {
                errorState = true; //just so the button will reset
                button1.Text = "Reset";
                textBox1.Text = @"1) Go to Google Calendar, and sign in with either your Google Account or Stony Brook's.
2) Once loaded, make a new calendar (if you want): Near My Calendars, click that small dropdown arrow, and click create a new calendar. Fill in whatever you want.
3) Optional: If you want to share your schedule with others, I recommend you use the Stony Brook account - everyone's names will autocomplete. Under""Share this Calendar"" in calendar details, add the names of people you want to see your calendar. That's it.
4) Back at calendar.google.com, under Other Calendars, click the arrow that has a dropdown menu with ""Import calendar"" - this will pop up a dialog asking for that file this program just made. Choose that file, and choose the calendar you want your schedule to go in.
5) Enjoy! If you want this calendar on your smartphone, you can find that on google.";
            }


        }
        bool errorState = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (errorState == true) {
                textBox1.Text = initialDirections;
                button1.Text = initialButtonText;
                errorState = false;
                return;
            }
            try {
                if (copyType != 0) {
                    convertClipboardType2(Clipboard.GetText(TextDataFormat.UnicodeText));
                } else {
                    convertClipboard(Clipboard.GetText(TextDataFormat.UnicodeText));
                }
            } catch (Exception ex) {
                errorState = true;
                textBox1.Text = ex.Message + "\n\nThis error probably means either you miscopied your schedule, or there's a bug."+
                " If it says access denied, it mean exactly that - choose a different file name and location. Email me at fam1995@gmail.com. Restart the app for the directions again.";
                button1.Text = "Reset";
                if (fout != null) {
                    fout.Close();
                }
            }
        }

        private void btnToggle_Click(object sender, EventArgs e) {
            if (copyType == 0) {
                btnToggle.Text = "This is the shopping cart copy.";
                copyType = 1;
            } else {
                btnToggle.Text = "This is my weekly schedule.";
                copyType = 0;
            }
        }
    }

    struct sbClassString {
        public string name;
        public string number;
        public string section;
        public string component;
        public string dateTime;
        public string location;
        public string instructors;
        public string startEndDate;
        public string fullName() {
            return name + " (" + component + " " + section + ")";
        }
        public override string ToString() {
            return fullName();
        }
    }
}
