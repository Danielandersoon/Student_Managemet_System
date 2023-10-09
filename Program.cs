using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;

namespace Student_Managemet_System
{
    public class StudentRecord {
        //
        // attributes
        //
        public string s_Name { get; set; }
        public string s_StudentID { get; set; }
        public List<List<int>> i_StudentScores { get; set; }
        public bool b_Passed { get; set; }

        //
        // Methods
        //
        public void generateNew(ref List<StudentRecord> studentRecords)
        {
            Random rnd = new Random(); // Random class may not be ideal as it has a set number of rndnum tables


            int iNumberofLoops = 1;
            for (int i = 0; i < iNumberofLoops; i++)
            {
                s_StudentID = "";
                // Make new student id 8 charachters long 
                for (int x = 0; x < 8; x++)
                {
                    s_StudentID = s_StudentID + rnd.Next(9);
                }

                // Itterate through array of all existing studentRecords looking for one that is the same as the new one
                // if it does already exist increase number of loops until its not the same
                for (int y = 0; y < studentRecords.Count; y++)
                {
                    if (studentRecords[y].getStudentID() == s_StudentID)
                    {
                        iNumberofLoops++;
                        break;
                    }
                }
            }

            // Set student name 
            Console.Write("Please input students name: ");
            setName(Console.ReadLine());

            i_StudentScores = new List<List<int>>();
        }
        //
        // Getters
        //
        public string getName() { return s_Name; }
        public string getStudentID() { return s_StudentID; }
        public List<List<int>> getStudentScores() {  return i_StudentScores; }

        public bool getPassed() { return b_Passed; }

        //
        // Setters
        //
        public void setName(string input) {
            s_Name = input;
        }


        // Creates new set of student scores
        public void addStudentScores(List<int> iNewSetOfScores) {
            // Adds new 1d array into the array of student arrays
            i_StudentScores.Add(iNewSetOfScores);

            // Updates the passed bool based on the avarage of the newest set of scores 
            updatePassed();
        }

        // Updates b_Passed based on avarage of newes set of scores
        public void updatePassed() {
            if (i_StudentScores[i_StudentScores.Count - 1].Average() < 40)
                b_Passed = false;
            else
                b_Passed = true;
        } 
    }

    public class CreateNewStudent {

        //
        // Methods
        //
        protected bool authorizePush(ConsoleKeyInfo keyIn, out bool authorized) {
            switch (keyIn.Key)
            {
                case ConsoleKey.Y:
                    {
                        authorized = true;
                        return true;
                    }
                default:
                    {   
                        authorized = false;
                        return false;
                    }
            }   
        }

        public StudentRecord Run(ref List<StudentRecord> studentRecords) {
            // Creates new instance of StudentRecord class
            StudentRecord record = new StudentRecord();

            bool bAuthorized = false;
            while (!bAuthorized) // Just stops the program moving on without validation
            {
                // call the generateNew method which does as it will
                record.generateNew(ref studentRecords);

                Console.Write("\nStudentID:    " + record.getStudentID() +
                              "\nStudent Name: " + record.getName() +
                              "\nIs this correct?(y/n): ");
                authorizePush(Console.ReadKey(), out bAuthorized);

                Console.WriteLine("\n");
            }
            return record;
        }

    }

    public class CreateNewStudentScores {

        //
        // attributes
        //
        protected int i_SelectedStudent;

        //
        // Methods
        //
        // searches array of student records for the selected student ID returns null if not found 
        protected int? findStudent(ref List<StudentRecord> studentRecords) {

            string sInputID;

            Console.Write("Please enter student ID: ");
            sInputID = Console.ReadLine();

            for (int x = 0; x < studentRecords.Count; x++)
            {
                if (sInputID == studentRecords[x].getStudentID())
                {
                    return x;
                }
            }
            return null;
        }

        // Just some valiodation for the scores to make sure it is an integer within range. 
        protected bool validateInputIsDigit(string input, out int iScore)
        {
            // Done with a try catch of a Convert.ToInt32 because regex is too hard
            try
            {
                iScore = Convert.ToInt32(input);
                if (iScore < 0 || iScore > 100)
                    return false;
                else 
                    return true;
            }
            catch
            {
                iScore = 0;
                return false;
            }
        }

        protected void setScoresLoop(ref List<StudentRecord> studentRecord)
        {
            // array of integers, this will be added to a new location in the first dimension of the students score array
            List<int> iScoresIn = new List<int>();
            bool bValidInput;

            // Takes and validates 6 inputs which are added to iScoresIn 
            for (int x = 0; x < 6; x++)
            {
                bValidInput = false;
                int iCurrentScore = 0;
                while (!bValidInput)
                {
                    Console.Write("Please input Score: ");
                    bValidInput = validateInputIsDigit(Console.ReadLine(), out iCurrentScore);
                }
                iScoresIn.Add(iCurrentScore);
            }

            // Ammend Scores array
            studentRecord[i_SelectedStudent].addStudentScores(iScoresIn);
        }

        protected void outputNewScores(ref List<StudentRecord> studentRecord)
        {
            // Gets the array of scores and grabs the new set weve just added
            List<List<int>> iScores = studentRecord[i_SelectedStudent].getStudentScores();

            int iLengthOfFirstDimension = iScores.Count - 1;
            int iLengthOfSecondDimension = iScores[iLengthOfFirstDimension].Count - 1;

            // itterate through the new set of scores and write to console
            for (int x = 0; x < iLengthOfSecondDimension + 1; x++)
            {
                Console.WriteLine((x + 1) +". " + iScores[iLengthOfFirstDimension][x]);
            }
        }

        protected CreateNewStudentScores() { } // just a blank constructor to prevent override issues in Update scores
        public CreateNewStudentScores(ref List<StudentRecord> studentRecords) {

            bool bStudentFound = false;
            int? iSelectedStudentNullable = null;

            while (!bStudentFound) 
            {
                iSelectedStudentNullable = findStudent(ref studentRecords);
                if (iSelectedStudentNullable!= null)
                    bStudentFound = true;
            }

            this.i_SelectedStudent = iSelectedStudentNullable.Value;

            setScoresLoop(ref studentRecords);
            outputNewScores(ref studentRecords);

        }
    }

    public class UpdateScores : CreateNewStudentScores {

        public UpdateScores(ref List<StudentRecord> studentRecords) : base()
        {
            bool bStudentFound = false;
            int? iSelectedStudentNullable = null;

            while (!bStudentFound)
            {
                iSelectedStudentNullable = findStudent(ref studentRecords);
                if (iSelectedStudentNullable != null)
                    bStudentFound = true;
            }

            this.i_SelectedStudent = iSelectedStudentNullable.Value;
            
            if (studentRecords[this.i_SelectedStudent].getStudentScores().Count == 0)
            {

                Console.WriteLine("No existing records found.\n");
                return; 
            }
            setScoresLoop(ref studentRecords);
            outputNewScores(ref studentRecords);      
        }
    }
    
    public class ViewStudentsRecords
    {
        protected int i_SelectedStudent;


        // searches array of student records for the selected student ID returns null if not found 
        protected int? findStudent(ref List<StudentRecord> studentRecords)
        {

            string sInputID;

            Console.Write("Please enter student ID: ");
            sInputID = Console.ReadLine();

            for (int x = 0; x < studentRecords.Count; x++)
            {
                if (sInputID == studentRecords[x].getStudentID())
                {
                    return x;
                }
            }
            return null;
        }

        protected void outputNewScores(ref List<StudentRecord> studentRecord)
        {
            // Gets the array of scores and grabs the new set weve just added
            List<List<int>> iScores = studentRecord[i_SelectedStudent].getStudentScores();

            int iLengthOfFirstDimension = iScores.Count - 1;
            if ((iLengthOfFirstDimension + 1) != 0)
            {
                int iLengthOfSecondDimension = iScores[iLengthOfFirstDimension].Count - 1;

                // itterate through the new set of scores and write to console
                for (int x = 0; x < iLengthOfSecondDimension + 1; x++)
                {
                    Console.WriteLine("score No. " + (x + 1) + ") " + iScores[iLengthOfFirstDimension][x]);
                }
            }

            // Checks if the student is passing then prints accordingly
            Console.WriteLine();
            if (studentRecord[i_SelectedStudent].getPassed())
                Console.WriteLine("Passing\n");
            else
                Console.WriteLine("Failling\n");

        }

        // Just uses getters from the studentRecord class to output the basic identifiers for the student
        protected void outputStudentDetails(ref List<StudentRecord> studentRecord)
        {
            Console.WriteLine("Student Name: " + studentRecord[i_SelectedStudent].getName());
            Console.WriteLine("Student ID: " + studentRecord[i_SelectedStudent].getStudentID() + "\n");
        }

        public ViewStudentsRecords(ref List<StudentRecord> studentRecords)
        {

            bool bStudentFound = false;
            int? iSelectedStudentNullable = null;

            while (!bStudentFound)
            {
                iSelectedStudentNullable = findStudent(ref studentRecords);
                if (iSelectedStudentNullable != null)
                    bStudentFound = true;
            }

            this.i_SelectedStudent = iSelectedStudentNullable.Value;

            outputStudentDetails(ref studentRecords);
            outputNewScores(ref studentRecords);

        }
    }

    public class MainMenu {

        //
        // attributes
        //

        private int i_Selection;

        private void displayOptions() 
        {
            Console.WriteLine("1) Create new student record\n" +
                              "2) Enter marks for student\n" +
                              "3) Update studnets mark\n" +
                              "4) Show student record\n" +
                              "5) Save + Quit program\n");
        }

        private bool takeInput(ConsoleKeyInfo keyIn)
        {
            // Switch case processess the key input and 'maps' it to an integer value (only realy for readability )
            switch (keyIn.Key)
            {
                case ConsoleKey.D1:
                    {
                        i_Selection = 1;
                        return true;
                    }
                case ConsoleKey.D2:
                    {
                        i_Selection = 2;
                        return true;
                    }
                case ConsoleKey.D3:
                    {
                        i_Selection = 3;
                        return true;
                    }
                case ConsoleKey.D4:
                    {
                        i_Selection = 4;
                        return true;
                    }
                case ConsoleKey.D5:
                    {
                        i_Selection = 5;
                        return true;
                    }
                default:
                    {
                        Console.WriteLine("\nThis is not a valid input please try again.\n\n");
                        return false;
                    }

            }
        }

        public int takeAndProcessSelection() {
            
            bool bSelectionValid = false;

            while (!bSelectionValid) // prevents program moving on without a valid selection
            {
                displayOptions(); // i feel the method name is self explanitory
                bSelectionValid = takeInput(Console.ReadKey()); 
            }
            Console.WriteLine("\n");
            return i_Selection;
        }
        
    }

    public class Application
    {
        //
        // attributes
        //
        private bool b_Quit = false;
        private int i_Selection;
        private const string c_s_SaveFileName = "StudentRecords.json";
        private List<StudentRecord> _studentRecords = new List<StudentRecord>();
        private List<string> s_StudentRecordJsons = new List<string>();
        private MainMenu menu;

        //
        // Methods
        //
        public Application() {

            // Load the csv and break it into individual json strings
            // File may not exist so try to load it and if not possible then crate it
            if (File.Exists(c_s_SaveFileName))
            {
                string sJsonRead = File.ReadAllText(c_s_SaveFileName);
                _studentRecords = JsonSerializer.Deserialize<List<StudentRecord>>(sJsonRead);
            }
            else  
            {
                FileStream file = File.Create(c_s_SaveFileName);
                file.Close();
            }

            // Create an instance of the main menu
            menu = new MainMenu(); 
        }

        protected void actionSelection(){

            // Switch case to call correct option
            switch (i_Selection)
            {
                case 1:
                    {
                        CreateNewStudent createNewStudent = new CreateNewStudent();
                        
                        // createNewStudent.Run creates a new StudentRecord which we ammend the array of student records with
                        _studentRecords.Add(createNewStudent.Run(ref _studentRecords));
                        break;
                    }
                case 2:
                    {
                        // Create NewStudentScores creates a 2d aray of student scores
                        CreateNewStudentScores createNewStudentScores = new CreateNewStudentScores(ref _studentRecords);
                        break;
                    }
                case 3:
                    {
                        // Create NewStudentScores ammends the 2d aray of student scores
                        UpdateScores updateScores = new UpdateScores(ref _studentRecords);
                        break;
                    }
                case 4: 
                    {
                        // Create ViewStudentRecords instance to output details about the selected student
                        ViewStudentsRecords viewStudentsRecords = new ViewStudentsRecords(ref _studentRecords);
                        break;
                    }
                case 5:
                    {
                        // Make a string storing a Json and write this to a .JSON file
                        // After set quit to true which breaks the application loop
                        string sJsonString = JsonSerializer.Serialize<List<StudentRecord>>(_studentRecords);
                        File.WriteAllText(c_s_SaveFileName, sJsonString);
                        b_Quit = true;
                        break;
                    }
                default: 
                    {
                        break;
                    }
            }
        }

        public void Run()
        {

            while (!b_Quit)
            {
                // Take user input and convert to int
                i_Selection = menu.takeAndProcessSelection();

                // Do the thing
                actionSelection();
            }

        }
    }

    internal class Program
    {
        // Entry point for program
        static void Main(string[] args)
        {
            Application application = new Application();
            application.Run();
        }
    }
}
