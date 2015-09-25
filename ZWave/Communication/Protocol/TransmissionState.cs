using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Communication.Protocol
{
    enum TransmissionState : byte
    {
        // Transmission complete and ACK received
        OK = 0x00,
        // Transmission complete, no ACK received,
        NoAck = 0x01,
        // Transmission failed
        Fail = 0x02,
        // Transmission failed, network busy
        NotIdle = 0x03,
        // Tranmission complete, no return route"
        CompleteNoRoute = 0x04,
    }
}
