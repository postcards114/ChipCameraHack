﻿using FFXIVUtil;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CameraHackTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ProcessModel> AllSelectedProcesses { get; } = new List<ProcessModel>();
        private ProcessModel HighlightedProcess;
 
        public MainWindow()
        {
            InitializeComponent();

            // uncomment to test many processes
            //AllSelectedProcesses.Add(new ProcessModel { Name = "1", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "2", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "3", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "4", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "5", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "6", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "7", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "8", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "9", Running = false });
            //AllSelectedProcesses.Add(new ProcessModel { Name = "10", Running = false });

            ListBox_RunningProcesses.DataContext = AllSelectedProcesses;
        }

        private void Button_DoTheThing_Click(object sender, RoutedEventArgs e)
        {
            foreach (var proc in AllSelectedProcesses)
            {
                StartProcess(proc);
            }

            ListBox_RunningProcesses.Items.Refresh();
        }

        private void Button_LoadProcess_Click(object sender, RoutedEventArgs e)
        {
            ProcessSelection processSelection = new ProcessSelection();
            Nullable<bool> dialogResult = processSelection.ShowDialog();
            if (dialogResult == true)
            {
                if (processSelection.NewSelectedProcesses != null)
                {
                    foreach (var SelectedProcess in processSelection.NewSelectedProcesses)
                    {
                        SelectedProcess.Hooked = true;
                        AllSelectedProcesses.Add(SelectedProcess);
                    }
                    ListBox_RunningProcesses.Items.Refresh();
                }
            }
        }

        private void StartProcess(ProcessModel proc)
        {
            if (proc.Running == false)
            {
                Memory.RunCameraHack(proc.Process);
                proc.Running = true;
            }
        }

        private void StopProcess(ProcessModel proc)
        {
            if (proc.Running == true)
            {
                Memory.StopCameraHack(proc.Process);
                proc.Running = false;
            }
        }

        private void RemoveProcess(ProcessModel proc)
        {
            StopProcess(proc);
            proc.Hooked = false;
            AllSelectedProcesses.Remove(AllSelectedProcesses.Where(x => x.Process.Id == proc.Process.Id).ToList()[0]);
            proc = null;
        }

        private void Button_StopProcess_Click(object sender, RoutedEventArgs e)
        {
            if (HighlightedProcess != null)
            {
                StopProcess(HighlightedProcess);
                ListBox_RunningProcesses.Items.Refresh();
            }
        }

        private void Button_RemoveProcess_Click(object sender, RoutedEventArgs e)
        {
            if (HighlightedProcess != null)
            {
                RemoveProcess(HighlightedProcess);
                ListBox_RunningProcesses.Items.Refresh();
            }
        }

        private void ListBox_RunningProcesses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                HighlightedProcess = (e.AddedItems[0] as ProcessModel);
            }
        }

        private void Button_StopAllProcesses_Click(object sender, RoutedEventArgs e)
        {
            foreach (var proc in AllSelectedProcesses)
            {
                StopProcess(proc);
            }

            ListBox_RunningProcesses.Items.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var proc in AllSelectedProcesses)
            {
                StopProcess(proc);
            }

            ListBox_RunningProcesses.Items.Refresh();
        }

        private void ListBox_RunningProcesses_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
            {
                HighlightedProcess = ((sender as ListBox).SelectedItem as ProcessModel);
                if (HighlightedProcess.Running)
                {
                    StopProcess(HighlightedProcess);
                }
                else
                {
                    StartProcess(HighlightedProcess);
                }

                ListBox_RunningProcesses.Items.Refresh();
            }
        }
    }
}
