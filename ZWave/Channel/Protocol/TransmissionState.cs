using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    enum TransmissionState : byte
    {
        /// <summary> 
        /// Successfully 
        /// </summary> 
        CompleteOk = 0x00,
        /// <summary> 
        /// No acknowledge is received before timeout from the destination node.  
        /// Acknowledge is discarded in case it is received after the timeout. 
        /// </summary> 
        CompleteNoAcknowledge = 0x01,
        /// <summary> 
        /// Not possible to transmit data because the Z-Wave network is busy (jammed). 
        /// </summary> 
        CompleteFail = 0x02,
        /// <summary> 
        /// no route found in Assign Route  
        /// </summary> 
        CompleteNoRoute = 0x04,
        /// <summary> 
        /// No Communication ACK received 
        /// </summary> 
        NoAcknowledge = 0x05,
        /// <summary> 
        /// No response received 
        /// </summary> 
        ResMissing = 0x06, 
    }
}
