/*
* ################################################################
*
* ProActive: The Java(TM) library for Parallel, Distributed,
*            Concurrent computing with Security and Mobility
*
* Copyright (C) 1997-2009 INRIA/University of Nice-Sophia Antipolis
* Contact: proactive@ow2.org
*
* This library is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; version 3 of
* the License.
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this library; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
* 
* If needed, contact us to obtain a release under GPL Version 2. 
*
*  Initial developer(s):               The ProActive Team
*                        http://proactive.inria.fr/team_members.htm
*  Contributor(s): ActiveEon Team - http://www.activeeon.com
*
* ################################################################
* $$ACTIVEEON_CONTRIBUTOR$$
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;

namespace ProActiveAgent
{
    /// <summary>    
    /// This class is used to limit the cpu usage of a set of processes (ie process throttling). 
    /// By adding processes to the watch list the sum of their cpu usages can be limited from 1 to 100%.
    /// In other words if a process A, B and C (with respective usages of 25%) are added
    /// to the watch list and the limit is set to 30%, it will become 10% per process.      
    /// On multi core processors the percentage is automatically translated to number of cores based percentage.
    /// On Windows XP setting process priority to RealTime makes it invulnerable to cpu usage limit.
    /// The cpu usage limit works only on Windows XP, Windows 2003 and Vista.    
    /// </summary>
    class CPULimiter
    {
        // Works on Windows XP, 2003 and Vista only
        [DllImport("ntdll.dll", EntryPoint = "NtResumeProcess", SetLastError = true)]
        private static extern uint NtResumeProcess(IntPtr processHandle);

        [DllImport("ntdll.dll", EntryPoint = "NtSuspendProcess", SetLastError = true)]
        private static extern uint NtSuspendProcess(IntPtr processHandle);
        /// <summary>
        /// see if 1sec based rate in ms is not too often </summary>
        public const int REFRESH_RATE_BASE_MS = 100;
        /// <summary>
        /// The list of process watchers.</summary>
        private readonly List<ProcessWatcher> watchList;
        // no need to be volatile since if an update is missed 
        // the timer cycle is fast enough to update the new value next cycle
        /// <summary>
        /// The maximum allowed cpu usage that will be shared between processes in the watch list.</summary>
        private float maxCpuUsagePercetage;
        /// <summary>
        /// The instance of System.Timers.Timer class used here instead of Windows.Forms.Timer since it
        /// can be runned inside a windows service.</summary>
        private readonly Timer watchingTimer;

        /// <summary>
        /// Creates a new instance of this class.</summary>
        public CPULimiter()
        {
            // Initialize the list
            this.watchList = new List<ProcessWatcher>();
            // Convert to processor count based percentage default is 100 * nb cores
            this.maxCpuUsagePercetage = 100 * Environment.ProcessorCount;
            // Create the watching timer
            this.watchingTimer = new Timer();
            // Hook up the Elapsed event for the timer.
            this.watchingTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            // Set the timer refresh rate
            this.watchingTimer.Interval = REFRESH_RATE_BASE_MS;
        }

        /// <summary>
        /// Sets a new max cpu usage that the processes in the watch list will share.
        /// </summary>
        /// <param name="maxCpuUsagePercentageBase100">The new percentage between 1 and 100</param>
        public void setNewMaxCpuUsage(uint maxCpuUsagePercentageBase100)
        {
            // Convert to processor count based percentage
            this.maxCpuUsagePercetage = maxCpuUsagePercentageBase100 * Environment.ProcessorCount;
        }

        /// <summary>
        /// Adds a new process to the watch list, added process will limited according to the allowed max cpu usage.
        /// </summary>
        /// <param name="processToWatch">The process to watch</param>
        public void addProcessToWatchList(Process processToWatch)
        {
            // Stop the timer...
            this.watchingTimer.Enabled = false;
            // Add this watcher to the watch list
            this.watchList.Add(new ProcessWatcher(processToWatch));
            // Restart the timer       
            this.watchingTimer.Enabled = true;
        }

        /// <summary>
        /// Clears the watch list, the processes that were in the watch list are no more limited.
        /// </summary>        
        public void clearWatchList()
        {
            // Stop the timer...
            this.watchingTimer.Enabled = false;
            // Clear the watch list and for each watcher resume its process if it is suspended
            for (int i = this.watchList.Count; i-- > 0; )
            {
                ProcessWatcher processWatcher = this.watchList[i];
                // If the process is suspended resume it
                if (processWatcher.isSuspended)
                {
                    NtResumeProcess(processWatcher.watchedProcess.Handle);
                }
                // Remove this process watcher from the watch list
                this.watchList.RemoveAt(i);
            }
        }

        // Executed by a timer every REFRESH_RATE_BASE_MS         
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Manually stop the timer...            
            this.watchingTimer.Enabled = false;
            // The total cpu usage
            float totalCpuUsage = 0;
            // Get the sum of cpu usage of all processes
            for (int i = this.watchList.Count; i-- > 0; )
            {
                ProcessWatcher processWatcher = this.watchList[i];
                // Check if the process has exited
                if (processWatcher.watchedProcess.HasExited)
                {
                    // Remove this process watcher from the watch list
                    this.watchList.RemoveAt(i);
                }
                else
                {
                    // If the process is suspended resume it
                    if (processWatcher.isSuspended)
                    {
                        // Re init the perf counter since the process was suspended
                        processWatcher.performanceCounter.NextValue();
                        uint res = NtResumeProcess(processWatcher.watchedProcess.Handle);
                        if (res == 0)
                        {
                            processWatcher.isSuspended = false;
                        }
                        else
                        {
                            // Something bad happended so remove this watcher
                            this.watchList.RemoveAt(i);
                        }
                    }
                    else
                    {
                        totalCpuUsage += processWatcher.performanceCounter.NextValue();
                    }
                }
            }

            int nextInterval = REFRESH_RATE_BASE_MS;

            // If max allowed cpu usage has been reached
            if (totalCpuUsage > this.maxCpuUsagePercetage)
            {
                for (int i = this.watchList.Count; i-- > 0; )
                {
                    ProcessWatcher processWatcher = this.watchList[i];
                    // Check if the process has exited
                    if (processWatcher.watchedProcess.HasExited)
                    {
                        // Remove this process watcher from the watch list
                        this.watchList.RemoveAt(i);
                    }
                    else
                    {
                        // Since some process watchers can already be suspended we need to check the state
                        if (!processWatcher.isSuspended)
                        {
                            // Suspend the process
                            uint res = NtSuspendProcess(processWatcher.watchedProcess.Handle);
                            if (res == 0)
                            {
                                processWatcher.isSuspended = true;
                            }
                            else
                            {
                                // Something bad happended so remove this watcher                                
                                this.watchList.RemoveAt(i);
                            }
                        }
                    }
                }
                // Compute the time to wait in order to reach the given percentage
                // formula : TimeToWait = BASE * ((previous_percentage / wanted_percentage) - 1)
                nextInterval = Convert.ToInt32(REFRESH_RATE_BASE_MS * ((totalCpuUsage / this.maxCpuUsagePercetage) - 1));
                if (nextInterval < 20)
                {
                    nextInterval = 20;
                }
            }

            // Change the next start            
            this.watchingTimer.Interval = nextInterval;
            this.watchingTimer.Enabled = true;
        }

        private class ProcessWatcher
        {
            private readonly Process _watchedProcess;
            private readonly PerformanceCounter _performanceCounter;
            private bool _isSuspended;

            public ProcessWatcher(Process processToWatch)
            {
                this._watchedProcess = processToWatch;
                // Build the instance name from the pid to get #nb based instance name
                string instanceName = GetProcessInstanceName(processToWatch.Id);
                this._performanceCounter = new PerformanceCounter(
                          "Process",
                          "% Processor Time",
                          instanceName, true);
                // Init the perf counter 
                this._performanceCounter.NextValue();
                this._isSuspended = false;
            }

            public Process watchedProcess
            {
                get { return this._watchedProcess; }
            }

            public PerformanceCounter performanceCounter
            {
                get { return this._performanceCounter; }
            }

            public bool isSuspended
            {
                get { return this._isSuspended; }
                set { this._isSuspended = value; }
            }

            private static string GetProcessInstanceName(int pid)
            {
                PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

                string[] instances = cat.GetInstanceNames();
                foreach (string instance in instances)
                {

                    using (PerformanceCounter cnt = new PerformanceCounter("Process",
                         "ID Process", instance, true))
                    {
                        if ((int)cnt.RawValue == pid)
                        {
                            return instance;
                        }
                    }
                }
                throw new Exception("Could not find performance counter " +
                    "instance name for current process. This is truly strange ...");
            }

        }

        // For test purpose
        static void Main0(string[] args)
        {
            Process[] par = Process.GetProcessesByName("ProcessLauncher");
            CPULimiter cpuLimiter = new CPULimiter();

            cpuLimiter.addProcessToWatchList(par[0]);

            cpuLimiter.setNewMaxCpuUsage(10);

            // Wait a little
            System.Threading.Thread.Sleep(20000);

            cpuLimiter.addProcessToWatchList(par[1]);

            System.Threading.Thread.Sleep(10000);

            cpuLimiter.setNewMaxCpuUsage(20);

            System.Threading.Thread.Sleep(10000);

            // Clear the watch list
            cpuLimiter.clearWatchList();

            // processes should be at 100% !each! during 10 secs
            System.Threading.Thread.Sleep(10000);

            // add the processes again
            cpuLimiter.addProcessToWatchList(par[0]);
            cpuLimiter.addProcessToWatchList(par[1]);

            // should be limited to 20% 
            System.Threading.Thread.Sleep(10000);

            cpuLimiter.clearWatchList();

            Console.ReadLine();
        }
    }
}