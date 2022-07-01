using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteStateMachine
{
    /*
     * https://www.eet-china.com/mp/a43466.html
     * Credits to the above mention post, thanks a ton!
     * 
     */

    /// <summary>
    /// 门的状态
    /// </summary>
    public enum Status
    {
        Locked = 1,
        Unlocked = 2,
        Closed = 3,
        Opened = 4
    }

    public enum EventSource : uint
    {
        Lock_Unlock = 0x0001,
        Unlock_Open = 0x0002,
        Unlock_Lock = 0x0003,

        Open_Close = 0x0004,
        Close_Lock = 0x0005,
        Close_Open = 0x0006
    }

    internal class StateModel
    {
        public Status status { get; set; }  
        public Status nextstatus { get; set; }  

        public Action action { get; set; }      // 事件发生后 -> 执行行为 -> 状态变更 
    }

    internal class VM
    {
        public StateModel state { get; set; }

        public VM(StateModel state)
        {
            this.state = state;
        }

        public EventSource CheckEvent()
        {
            Console.WriteLine("\n");
            Console.WriteLine("\tCurrent Status:\t" + state.status.ToString());
            Console.WriteLine("\tNext Status:\t" + state.nextstatus.ToString());
            Console.Write("input>");
            return (EventSource)Convert.ToUInt32(Console.ReadLine()); // ignore user input exception,I'm lazy
        }


        private Dictionary<int, int[]> keyValuePairs = new Dictionary<int, int[]>()
        {
            { 1,new[] { 1} },           // lock       unlock
            { 2,new[] { 2,3} },         // unlock       open,lock
            { 3,new[] { 5,6} },         // closed       lock,open
            { 4,new[] { 4} }            // open         close
        };

        /// <summary>
        /// 根据事件 + 当前状态 确定下一个状态是什么
        /// 筛选不正常的操作
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Status BerealNextStatus(EventSource source)
        {
            if (!keyValuePairs.Where(x => x.Key.Equals((int)state.status)).FirstOrDefault().Value.ToList().Contains((int)source))
            {
                Console.Error.WriteLine("输入的事件不符合当前状态");
                return state.nextstatus; // 返回原本的状态
            }

            switch (state.status)
            {
                case Status.Locked:
                    return Status.Unlocked;
                    break;
                case Status.Unlocked:
                    if(source is EventSource.Unlock_Open)
                    {
                        return Status.Opened;
                    }
                    return Status.Locked;
                    break;
                case Status.Closed:
                    if (source is EventSource.Close_Lock)
                    {
                        return Status.Locked;
                    }
                    return Status.Opened;
                    break;
                case Status.Opened:
                    return Status.Closed;
                    break;
                default:
                    break;
            }
            return default;
        }


        public bool ChangeStatus(EventSource source)
        {
            var tmp = this.state.status;
            this.state.status = this.state.nextstatus;
            var intend = BerealNextStatus(source);
            if (intend != state.nextstatus)
            {
                this.state.nextstatus = intend;
                return true;
            }
            else
            {
                this.state.status = tmp;
                this.state.action = ActionMethod.StayStill;
                return false;
            }
        }

        public void OneDargon()
        {
            var result = CheckEvent();
            switch (result)
            {
                case EventSource.Lock_Unlock:
                    this.state.action = ActionMethod.LockToUnlock;
                    break;
                case EventSource.Unlock_Open:
                    this.state.action = ActionMethod.UnlockToOpen;
                    break;
                case EventSource.Unlock_Lock:
                    this.state.action = ActionMethod.UnlockToLock;
                    break;
                case EventSource.Open_Close:
                    this.state.action = ActionMethod.OpenToClose;
                    break;
                case EventSource.Close_Lock:
                    this.state.action = ActionMethod.CloseToLock;
                    break;
                case EventSource.Close_Open:
                    this.state.action = ActionMethod.CloseToOpen;
                    break;
                default:
                    break;
            }

            ChangeStatus(result);
            this.state.action(); // 先执行事件发生的行为  然后更改状态
        }

    }

    public class ActionMethod
    {
        public static void LockToUnlock()
        {
            Console.WriteLine("门从 lock -> unlock");
        }

        public static void UnlockToOpen()
        {
            Console.WriteLine("门从 unlock -> open");
        }

        public static void OpenToClose()
        {
            Console.WriteLine("门从 open -> close");
        }

        internal static void UnlockToLock()
        {
            Console.WriteLine("门从 unlock -> lock");
        }

        internal static void CloseToOpen()
        {
            Console.WriteLine("门从 close -> open");
        }

        internal static void CloseToLock()
        {
            Console.WriteLine("门从 close -> lock");
        }

        internal static void StayStill()
        {
            Console.WriteLine("保持状态");
        }
    }
}
