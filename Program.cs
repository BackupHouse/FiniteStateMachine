using System;

namespace FiniteStateMachine
{
    class Program : ActionMethod
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Finite state machine init...");
            Console.WriteLine(@"
请输入选择

        Lock_Unlock = 0x0001,
        Unlock_Open = 0x0002,
        Unlock_Lock = 0x0003,

        Open_Close = 0x0004,
        Close_Lock = 0x0005,
        Close_Open = 0x0006

");
            StateModel state = new StateModel()
            {
                status = Status.Locked,             // 上锁 下一个状态只能是解锁
                nextstatus = Status.Unlocked,
                action = LockToUnlock
            };
            VM vm = new VM(state);
            while (true)
            {
                vm.OneDargon();
            }
        }
    }
}
