using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core.CashCore
{
    public enum BVStatus
    {
        PowerUp = 0x10,
        PowerUpwithBillinValidator = 0x11,
        PowerUpwithBillinStacker = 0x12,
        Initialize = 0x13,
        Idling = 0x14,
        Accepting = 0x15,
        Stacking = 0x17,
        Returning = 0x18,
        UnitDisabled = 0x19,
        Holding = 0x1a,
        DeviceBusy = 0x1b,
        rejecting = 0x1c,
        DropCassetteFull = 0x41,
        DropCassetteoutofposition = 0x42,
        ValidatorJammed = 0x43,
        DropCassetteJammed = 0x44,
        Cheated = 0x45,
        Pause = 0x46,
        Failure = 0x47,
        EscrowPosition = 0x80,
        BillStacked = 0x81,
        BillReturned = 0x82,
    }

    public enum RejectingCode
    {
        Insertion = 0x60,
        Magnetic = 0x61,
        Remainedbillinhead = 0x62,
        Multiplying = 0x63,
        Conveying = 0x64,
        Identification1 = 0x65,
        Verification = 0x66,
        Optic = 0x67,
        Inhibit = 0x68,
        Capacity = 0x69,
        Operation = 0x6a,
        Length = 0x6c,
        unrecognised_barcode = 0x92,
        incorrect_number_of_characters_in_barcode = 0x93,
        unknown_barcode_start_sequence = 0x94,
        unknown_barcode_stop_sequence = 0x95,
    }

    public enum FailureCodes
    {
        StackMotorFailure = 0x50,
        TransportMotorSpeedFailure = 0x51,
        TransportMotorFailure = 0x52,
        AligningMotorFailure = 0x53,
        InitialCassetteStatusFailure = 0x54,
        OpticCanalFailure = 0x55,
        MagneticCanalFailure = 0x56,
        CapacitanceCanalFailure = 0x5f,
    }
    /// <summary>
    /// 币种
    /// </summary>
    public enum BillType
    {
        RMB1 = 0,
        RMB5 = 2,
        RMB10 = 3,
        RMB20 = 4,
        RMB50 = 5,
        RMB100 = 6,
    }
}
