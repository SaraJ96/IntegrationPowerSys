using System;
using System.Collections.Generic;
using System.Text;

namespace FTN.Common
{
	
	public enum DMSType : short
	{		
		MASK_TYPE							= unchecked((short)0xFFFF),

        TERMINAL                                = 0x0001,
        REGULATINGCONTROL                       = 0x0002,
        STATICVARCOMPENSATOR                    = 0x0003,
        SHUNTCOMPENSATOR                        = 0x0004,
        REACTIVECAPABILITYCURVE                 = 0x0005,
        SYNCHRONOUSMACHINE                      = 0x0006,
        CONTROL                                 = 0x0007,
    }

    [Flags]
	public enum ModelCode : long
	{
        IDOBJ                                       = 0x1000000000000000,
        IDOBJ_GID                                   = 0x1000000000000104,

        CONTROL                                     = 0x1100000000070000,
        CONTROL_REGULATINGCONDEQ                    = 0x1100000000070109,

        TERMINAL                                    = 0x1200000000010000,
        TERMINAL_REGULATINGCONTROL                  = 0x1200000000010119,

        POWERSYSTEMRESOURCE                         = 0x1300000000000000,

        CURVE                                       = 0x1400000000000000,

        REGULATINGCONTROL                           = 0x1310000000020000,
        REGULATINGCONTROL_DISCRETE                  = 0x1310000000020101,
        REGULATINGCONTROL_MODE                      = 0x131000000002020a,
        REGULATINGCONTROL_MONITOREDPHASE            = 0x131000000002030a,
        REGULATINGCONTROL_TARGETRANGE               = 0x1310000000020405,
        REGULATINGCONTROL_TARGETVALUE               = 0x1310000000020505,
        REGULATINGCONTROL_TERMINAL                  = 0x1310000000020609,
        REGULATINGCONTROL_REGULATINGCONDEQ          = 0x1310000000020719,

        EQUIPMENT                                   = 0x1320000000000000,
        EQUIPMENT_AGGREGATE                         = 0x1320000000000101,
        EQUIPMENT_NORMALLYLNSERVICE                 = 0x1320000000000201,

        REACTIVECAPABILITYCURVE                     = 0x1410000000050000,
        REACTIVECAPABILITYCURVE_SYNCHRONOUSMACHINE  = 0x1410000000050119,

        CONDUCTINGEQUIPMENT                         = 0x1321000000000000,

        REGULATINGCONDEQ                            = 0x1321100000000000,
        REGULATINGCONDEQ_REGULATINGCONTROL          = 0x1321100000000109,
        REGULATINGCONDEQ_CONTROLS                   = 0x1321100000000119,

        STATICVARCOMPENSATOR                        = 0x1321110000030000,

        SHUNTCOMPENSATOR                            = 0x1321120000040000,

        ROTATINGMACHINE                             = 0x1321130000000000,

        SYNCHRONOUSMACHINE                          = 0x1321131000060000,
        SYNCHRONOUSMACHINE_REACTIVECAPABILITYCURVES = 0x1311131000060109,

    }

    [Flags]
	public enum ModelCodeMask : long
	{
		MASK_TYPE			 = 0x00000000ffff0000,
		MASK_ATTRIBUTE_INDEX = 0x000000000000ff00,
		MASK_ATTRIBUTE_TYPE	 = 0x00000000000000ff,

		MASK_INHERITANCE_ONLY = unchecked((long)0xffffffff00000000),
		MASK_FIRSTNBL		  = unchecked((long)0xf000000000000000),
		MASK_DELFROMNBL8	  = unchecked((long)0xfffffff000000000),		
	}																		
}


