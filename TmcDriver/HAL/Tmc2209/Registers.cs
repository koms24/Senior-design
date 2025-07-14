using Iot.Device.Ft4222;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver.HAL.Tmc2209
{
    namespace Registers
    {
        public enum General
        {
            GCONF = 0x00,
            GSTAT,
            IFCNT,
            NODECONF,
            OTP_PROG,
            OTP_READ,
            IOIN,
            FACTORY_CONF
        }
        public enum Velocity
        {
            IHOLD_IRUN = 0x10,
            TPOWER_DOWN,
            TSTEP,
            TPWMTHRS,
            VACTUAL = 0x22
        }
        public enum StallGuard
        {
            TCOOLTHRS = 0x14,
            SGTHRS = 0x40,
            SG_RESULT,
            COOLCONF
        }
        public enum Sequencer
        {
            MSCNT = 0x6A,
            MSCURACT
        }
        public enum Chopper
        {
            CHOPCONF = 0x6C,
            DRV_STATUS = 0x6F,
            PWMCONF = 0x70,
            PWM_SCALE = 0x71,
            PWM_AUTO = 0x72
        }
        public struct GCONF
        {
            public const bool Readable = true;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            internal static readonly BitVector32.Section _I_scale_analog = BitVector32.CreateSection(0x1);
            internal static readonly BitVector32.Section _internal_Rsense = BitVector32.CreateSection(0x1, _I_scale_analog);
            internal static readonly BitVector32.Section _en_SpreadCycle = BitVector32.CreateSection(0x1, _internal_Rsense);
            internal static readonly BitVector32.Section _shaft = BitVector32.CreateSection(0x1, _en_SpreadCycle);
            internal static readonly BitVector32.Section _index_otpw = BitVector32.CreateSection(0x1, _shaft);
            internal static readonly BitVector32.Section _index_step = BitVector32.CreateSection(0x1, _index_otpw);
            internal static readonly BitVector32.Section _pdn_disable = BitVector32.CreateSection(0x1, _index_step);
            internal static readonly BitVector32.Section _mstep_reg_select = BitVector32.CreateSection(0x1, _pdn_disable);
            internal static readonly BitVector32.Section _multistep_filt = BitVector32.CreateSection(0x1, _mstep_reg_select);
            internal static readonly BitVector32.Section _test_mode = BitVector32.CreateSection(0x1, _multistep_filt);


            public GCONF()
            {
                data = new BitVector32(0);
            }

            public GCONF(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public GCONF(int v)
            {
                data = new BitVector32(v);
            }

            public byte IScaleAnalog {
                get { return (byte)data[_I_scale_analog]; }
                set { data[_I_scale_analog] = value; }
            }
            public byte InternalRsense
            {
                get { return (byte)data[_internal_Rsense]; }
                set { data[_internal_Rsense] = value; }
            }
            public byte EnSpreadCycle
            {
                get { return (byte)data[_en_SpreadCycle]; }
                set { data[_en_SpreadCycle] = value; }
            }
            public byte Shaft
            {
                get { return (byte)data[_shaft]; }
                set { data[_shaft] = value; }
            }
            public byte IndexOtpw
            {
                get { return (byte)data[_index_otpw]; }
                set { data[_index_otpw] = value; }
            }
            public byte IndexStep
            {
                get { return (byte)data[_index_step]; }
                set { data[_index_step] = value; }
            }
            public byte PdnDisable
            {
                get { return (byte)data[_pdn_disable]; }
                set { data[_pdn_disable] = value; }
            }
            public byte MstepRegSelect
            {
                get { return (byte)data[_mstep_reg_select]; }
                set { data[_mstep_reg_select] = value; }
            }
            public byte MultistepFilt
            {
                get { return (byte)data[_multistep_filt]; }
                set { data[_multistep_filt] = value; }
            }
            public byte TestMode
            {
                get { return (byte)data[_test_mode]; }
                set { data[_test_mode] = value; }
            }
        }

        public struct GSTAT
        {
            public const bool Readable = true;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public GSTAT()
            {
                data = new BitVector32(0);
            }

            public GSTAT(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public GSTAT(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _reset = BitVector32.CreateSection(0x1);
            internal static readonly BitVector32.Section _drv_err = BitVector32.CreateSection(0x1, _reset);
            internal static readonly BitVector32.Section _uv_cp = BitVector32.CreateSection(0x1, _drv_err);

            public byte Reset
            {
                get { return (byte)data[_reset]; }
                set { data[_reset] = value; }
            }
            public byte DrvErr
            {
                get { return (byte)data[_drv_err]; }
                set { data[_drv_err] = value; }
            }
            public byte UvCp
            {
                get { return (byte)data[_uv_cp]; }
                set { data[_uv_cp] = value; }
            }
        }

        public struct IFCNT
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public IFCNT()
            {
                data = new BitVector32(0);
            }

            public IFCNT(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public IFCNT(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _ifcnt = BitVector32.CreateSection(0xff);

            public byte Ifcnt
            {
                get { return (byte)data[_ifcnt]; }
                set { data[_ifcnt] = value; }
            }

        }

        public struct NODECONF
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public NODECONF()
            {
                data = new BitVector32(0);
            }

            public NODECONF(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public NODECONF(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _reserved = BitVector32.CreateSection(0xff);
            internal static readonly BitVector32.Section _nodeconf = BitVector32.CreateSection(0x0f, _reserved);

            public byte Nodeconf
            {
                get { return (byte)data[_nodeconf]; }
                set { data[_nodeconf] = value; }
            }
        }

        public struct OTP_PROG
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public OTP_PROG()
            {
                data = new BitVector32(0);
            }

            public OTP_PROG(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public OTP_PROG(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _otpbit = BitVector32.CreateSection(0x07);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x01, _otpbit);
            internal static readonly BitVector32.Section _otpbyte = BitVector32.CreateSection(0x03, _reserved0);
            internal static readonly BitVector32.Section _reserved1 = BitVector32.CreateSection(0x01, _otpbyte);
            internal static readonly BitVector32.Section _otpmagic = BitVector32.CreateSection(0xff, _reserved1);

            public byte Otpbit
            {
                get { return (byte)data[_otpbit]; }
                set { data[_otpbit] = value; }
            }
            public byte Otpbyte
            {
                get { return (byte)data[_otpbyte]; }
                set { data[_otpbyte] = value; }
            }
            public byte Otpmagic
            {
                get { return (byte)data[_otpmagic]; }
                set { data[_otpmagic] = value; }
            }
        }

        public struct OTP_READ
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public OTP_READ()
            {
                data = new BitVector32(0);
            }

            public OTP_READ(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public OTP_READ(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _otp0 = BitVector32.CreateSection(0xff);
            internal static readonly BitVector32.Section _otp1 = BitVector32.CreateSection(0xff, _otp0);
            internal static readonly BitVector32.Section _otp2 = BitVector32.CreateSection(0xff, _otp1);

            public byte Otp0
            {
                get { return (byte)data[_otp0]; }
                set { data[_otp0] = value; }
            }
            public byte Otp1
            {
                get { return (byte)data[_otp1]; }
                set { data[_otp1] = value; }
            }
            public byte Otp2
            {
                get { return (byte)data[_otp2]; }
                set { data[_otp2] = value; }
            }
        }

        public struct IOIN
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public IOIN()
            {
                data = new BitVector32(0);
            }

            public IOIN(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public IOIN(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _enn = BitVector32.CreateSection(0x01);
            internal static readonly BitVector32.Section _0_0 = BitVector32.CreateSection(0x01, _enn);
            internal static readonly BitVector32.Section _ms1 = BitVector32.CreateSection(0x01, _0_0);
            internal static readonly BitVector32.Section _ms2 = BitVector32.CreateSection(0x01, _ms1);
            internal static readonly BitVector32.Section _diag = BitVector32.CreateSection(0x01, _ms2);
            internal static readonly BitVector32.Section _0_1 = BitVector32.CreateSection(0x01, _diag);
            internal static readonly BitVector32.Section _pdn_uart = BitVector32.CreateSection(0x01, _0_1);
            internal static readonly BitVector32.Section _step = BitVector32.CreateSection(0x01, _pdn_uart);
            internal static readonly BitVector32.Section _spread_en = BitVector32.CreateSection(0x01, _step);
            internal static readonly BitVector32.Section _dir = BitVector32.CreateSection(0x01, _spread_en);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x3fff, _dir);
            internal static readonly BitVector32.Section _version = BitVector32.CreateSection(0xff, _reserved0);

            public byte Enn
            {
                get { return (byte)data[_enn]; }
                set { data[_enn] = value; }
            }
            public byte Gnd0
            {
                get { return (byte)data[_0_0]; }
                set { data[_0_0] = value; }
            }
            public byte Ms1
            {
                get { return (byte)data[_ms1]; }
                set { data[_ms1] = value; }
            }
            public byte Ms2
            {
                get { return (byte)data[_ms2]; }
                set { data[_ms2] = value; }
            }
            public byte Diag
            {
                get { return (byte)data[_diag]; }
                set { data[_diag] = value; }
            }
            public byte Gnd1
            {
                get { return (byte)data[_0_1]; }
                set { data[_0_1] = value; }
            }
            public byte PdnUart
            {
                get { return (byte)data[_pdn_uart]; }
                set { data[_pdn_uart] = value; }
            }
            public byte Step
            {
                get { return (byte)data[_step]; }
                set { data[_step] = value; }
            }
            public byte SpreadEn
            {
                get { return (byte)data[_spread_en]; }
                set { data[_spread_en] = value; }
            }
            public byte Dir
            {
                get { return (byte)data[_dir]; }
                set { data[_dir] = value; }
            }
            public byte Version
            {
                get { return (byte)data[_version]; }
                set { data[_version] = value; }
            }
        }

        public struct FACTORY_CONF
        {
            public const bool Readable = true;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public FACTORY_CONF()
            {
                data = new BitVector32(0);
            }

            public FACTORY_CONF(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public FACTORY_CONF(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _fclktrim = BitVector32.CreateSection(0x0f);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x07, _fclktrim);
            internal static readonly BitVector32.Section _ottrim = BitVector32.CreateSection(0x03, _reserved0);

            public byte Fclktrim
            {
                get { return (byte)data[_fclktrim]; }
                set { data[_fclktrim] = value; }
            }
            public byte Ottrim
            {
                get { return (byte)data[_ottrim]; }
                set { data[_ottrim] = value; }
            }
        }

        public struct IHOLD_IRUN
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public IHOLD_IRUN()
            {
                data = new BitVector32(0);
            }

            public IHOLD_IRUN(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public IHOLD_IRUN(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _ihold = BitVector32.CreateSection(0x1f);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x07, _ihold);
            internal static readonly BitVector32.Section _irun = BitVector32.CreateSection(0x1f, _reserved0);
            internal static readonly BitVector32.Section _reserved1 = BitVector32.CreateSection(0x07, _irun);
            internal static readonly BitVector32.Section _iholddelay = BitVector32.CreateSection(0x0f, _reserved1);

            public byte Ihold
            {
                get { return (byte)data[_ihold]; }
                set { data[_ihold] = value; }
            }
            public byte Irun
            {
                get { return (byte)data[_irun]; }
                set { data[_irun] = value; }
            }
            public byte Iholddelay
            {
                get { return (byte)data[_iholddelay]; }
                set { data[_iholddelay] = value; }
            }
        }

        public struct TPOWER_DOWN
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public TPOWER_DOWN()
            {
                data = new BitVector32(0);
            }

            public TPOWER_DOWN(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public TPOWER_DOWN(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _tpower_down = BitVector32.CreateSection(0xff);

            public byte TpowerDown
            {
                get { return (byte)data[_tpower_down]; }
                set { data[_tpower_down] = value; }
            }
        }

        public struct TSTEP
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public TSTEP()
            {
                data = new BitVector32(0);
            }

            public TSTEP(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public TSTEP(int v)
            {
                data = new BitVector32(v);
            }

            public int Tstep
            {
                get { return (int)(((uint)data.Data) & ((uint)0x000fffff)); }
                set { data = new BitVector32((int)(((uint)value) & ((uint)0x000fffff))); }
            }
        }

        public struct TPWMTHRS
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public TPWMTHRS()
            {
                data = new BitVector32(0);
            }

            public TPWMTHRS(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public TPWMTHRS(int v)
            {
                data = new BitVector32(v);
            }

            public int Tpwmthrs
            {
                get { return (int)(((uint)data.Data) & ((uint)0x000fffff)); }
                set { data = new BitVector32((int)(((uint)value) & ((uint)0x000fffff))); }
            }
        }

        public struct VACTUAL
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public VACTUAL()
            {
                data = new BitVector32(0);
            }

            public VACTUAL(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public VACTUAL(int v)
            {
                data = new BitVector32(v);
            }

            public int Vactual
            {
                get { return (int)(((uint)data.Data) & ((uint)0x00ffffff)); }
                set { data = new BitVector32((int)(((uint)value) & ((uint)0x00ffffff))); }
            }
        }

        public struct TCOOLTHRS
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public TCOOLTHRS()
            {
                data = new BitVector32(0);
            }

            public TCOOLTHRS(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public TCOOLTHRS(int v)
            {
                data = new BitVector32(v);
            }

            public int Tcoolthrs
            {
                get { return (int)(((uint)data.Data) & ((uint)0x000fffff)); }
                set { data = new BitVector32((int)(((uint)value) & ((uint)0x000fffff))); }
            }
        }

        public struct SGTHRS
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public SGTHRS()
            {
                data = new BitVector32(0);
            }

            public SGTHRS(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public SGTHRS(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _sgthrs = BitVector32.CreateSection(0xff);

            public byte Sgthrs
            {
                get { return (byte)data[_sgthrs]; }
                set { data[_sgthrs] = value; }
            }
        }

        public struct SG_RESULT
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public SG_RESULT()
            {
                data = new BitVector32(0);
            }

            public SG_RESULT(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public SG_RESULT(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _sg_result = BitVector32.CreateSection(0x03ff);

            public Int16 SgResult
            {
                get { return (Int16)data[_sg_result]; }
                set { data[_sg_result] = value; }
            }
        }

        public struct COOLCONF
        {
            public const bool Readable = false;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public COOLCONF()
            {
                data = new BitVector32(0);
            }

            public COOLCONF(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public COOLCONF(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section __byte_offset_0 = BitVector32.CreateSection(0xff);
            internal static readonly BitVector32.Section __byte_offset_1 = BitVector32.CreateSection(0xff, __byte_offset_0);

            internal static readonly BitVector32.Section _coolconf_0 = BitVector32.CreateSection(0xff);
            internal static readonly BitVector32.Section _coolconf_1 = BitVector32.CreateSection(0xff, _coolconf_0);

            internal static readonly BitVector32.Section _semin = BitVector32.CreateSection(0x0f);

            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x01, _semin);
            internal static readonly BitVector32.Section _seup = BitVector32.CreateSection(0x03, _reserved0);
            internal static readonly BitVector32.Section _reserved1 = BitVector32.CreateSection(0x01, _seup);

            internal static readonly BitVector32.Section _semax = BitVector32.CreateSection(0x0f, _reserved1);

            internal static readonly BitVector32.Section _reserved2 = BitVector32.CreateSection(0x01, _semax);
            internal static readonly BitVector32.Section _sedn = BitVector32.CreateSection(0x03, _reserved2);
            internal static readonly BitVector32.Section _seimin = BitVector32.CreateSection(0x01, _sedn);

            public Int16 Coolconf
            {
                get
                {
                    var t = new BitVector32(0);
                    t[__byte_offset_0] = data[_coolconf_0];
                    t[__byte_offset_1] = data[_coolconf_1];
                    return (Int16)t.Data;
                }
                set
                {
                    var t = new BitVector32(value);
                    data[_coolconf_0] = t[__byte_offset_0];
                    data[_coolconf_1] = t[__byte_offset_1];
                }
            }
            public byte Semin
            {
                get { return (byte)data[_semin]; }
                set { data[_semin] = value; }
            }
            public byte Seup
            {
                get { return (byte)data[_seup]; }
                set { data[_seup] = value; }
            }
            public byte Semax
            {
                get { return (byte)data[_semax]; }
                set { data[_semax] = value; }
            }
            public byte Sedn
            {
                get { return (byte)data[_sedn]; }
                set { data[_sedn] = value; }
            }
            public byte Seimin
            {
                get { return (byte)data[_seimin]; }
                set { data[_seimin] = value; }
            }
        }

        public struct MSCNT
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public MSCNT()
            {
                data = new BitVector32(0);
            }

            public MSCNT(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public MSCNT(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _mscnt = BitVector32.CreateSection(0x03ff);

            public Int16 Mscnt
            {
                get { return (Int16)data[_mscnt]; }
                set { data[_mscnt] = value; }
            }
        }

        public struct MSCURACT
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public MSCURACT()
            {
                data = new BitVector32(0);
            }

            public MSCURACT(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public MSCURACT(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _cur_b = BitVector32.CreateSection(0x01ff);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x7f, _cur_b);
            internal static readonly BitVector32.Section _cur_a = BitVector32.CreateSection(0x01ff, _reserved0);

            public Int16 CurB
            {
                get { return (Int16)data[_cur_b]; }
                set { data[_cur_b] = value; }
            }
            public Int16 CurA
            {
                get { return (Int16)data[_cur_a]; }
                set { data[_cur_a] = value; }
            }
        }

        public struct CHOPCONF
        {
            public const bool Readable = true;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public CHOPCONF()
            {
                data = new BitVector32(0);
            }

            public CHOPCONF(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public CHOPCONF(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _toff = BitVector32.CreateSection(0x0f);
            internal static readonly BitVector32.Section _hstrt = BitVector32.CreateSection(0x07, _toff);
            internal static readonly BitVector32.Section _hend = BitVector32.CreateSection(0x0f, _hstrt);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x0f, _hend);
            internal static readonly BitVector32.Section _tbl = BitVector32.CreateSection(0x03, _reserved0);
            internal static readonly BitVector32.Section _vsense = BitVector32.CreateSection(0x01, _tbl);
            internal static readonly BitVector32.Section _reserved1 = BitVector32.CreateSection(0x3f, _vsense);
            internal static readonly BitVector32.Section _mres = BitVector32.CreateSection(0x0f, _reserved1);
            internal static readonly BitVector32.Section _intpol = BitVector32.CreateSection(0x01, _mres);
            internal static readonly BitVector32.Section _dedge = BitVector32.CreateSection(0x01, _intpol);
            internal static readonly BitVector32.Section _diss2g = BitVector32.CreateSection(0x01, _dedge);
            internal static readonly BitVector32.Section _diss2vs = BitVector32.CreateSection(0x01, _diss2g);

            public byte Toff
            {
                get { return (byte)data[_toff]; }
                set { data[_toff] = value; }
            }
            public byte Hstrt
            {
                get { return (byte)data[_hstrt]; }
                set { data[_hstrt] = value; }
            }
            public byte Hend
            {
                get { return (byte)data[_hend]; }
                set { data[_hend] = value; }
            }
            public byte Tbl
            {
                get { return (byte)data[_tbl]; }
                set { data[_tbl] = value; }
            }
            public byte Vsense
            {
                get { return (byte)data[_vsense]; }
                set { data[_vsense] = value; }
            }
            public byte Mres
            {
                get { return (byte)data[_mres]; }
                set { data[_mres] = value; }
            }
            public byte Intpol
            {
                get { return (byte)data[_intpol]; }
                set { data[_intpol] = value; }
            }
            public byte Dedge
            {
                get { return (byte)data[_dedge]; }
                set { data[_dedge] = value; }
            }
            public byte Diss2g
            {
                get { return (byte)data[_diss2g]; }
                set { data[_diss2g] = value; }
            }
            public byte Diss2vs
            {
                get { return (byte)data[_diss2vs]; }
                set { data[_diss2vs] = value; }
            }
        }

        public struct DRV_STATUS
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public DRV_STATUS()
            {
                data = new BitVector32(0);
            }

            public DRV_STATUS(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public DRV_STATUS(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _otpw = BitVector32.CreateSection(0x01);
            internal static readonly BitVector32.Section _ot = BitVector32.CreateSection(0x01, _otpw);
            internal static readonly BitVector32.Section _s2ga = BitVector32.CreateSection(0x01, _ot);
            internal static readonly BitVector32.Section _s2gb = BitVector32.CreateSection(0x01, _s2ga);
            internal static readonly BitVector32.Section _s2vsa = BitVector32.CreateSection(0x01, _s2gb);
            internal static readonly BitVector32.Section _s2vsb = BitVector32.CreateSection(0x01, _s2vsa);
            internal static readonly BitVector32.Section _ola = BitVector32.CreateSection(0x01, _s2vsb);
            internal static readonly BitVector32.Section _olb = BitVector32.CreateSection(0x01, _ola);
            internal static readonly BitVector32.Section _t120 = BitVector32.CreateSection(0x01, _olb);
            internal static readonly BitVector32.Section _t143 = BitVector32.CreateSection(0x01, _t120);
            internal static readonly BitVector32.Section _t150 = BitVector32.CreateSection(0x01, _t143);
            internal static readonly BitVector32.Section _t157 = BitVector32.CreateSection(0x01, _t150);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x0f, _t157);
            internal static readonly BitVector32.Section _cs_actual = BitVector32.CreateSection(0x1f, _reserved0);
            internal static readonly BitVector32.Section _reserved1 = BitVector32.CreateSection(0x07, _cs_actual);
            internal static readonly BitVector32.Section _reserved2 = BitVector32.CreateSection(0x3f, _reserved1);
            internal static readonly BitVector32.Section _stealth = BitVector32.CreateSection(0x01, _reserved2);
            internal static readonly BitVector32.Section _stst = BitVector32.CreateSection(0x01, _stealth);

            public byte Otpw
            {
                get { return (byte)data[_otpw]; }
                set { data[_otpw] = value; }
            }
            public byte Ot
            {
                get { return (byte)data[_ot]; }
                set { data[_ot] = value; }
            }
            public byte S2ga
            {
                get { return (byte)data[_s2ga]; }
                set { data[_s2ga] = value; }
            }
            public byte S2gb
            {
                get { return (byte)data[_s2gb]; }
                set { data[_s2gb] = value; }
            }
            public byte S2vsa
            {
                get { return (byte)data[_s2vsa]; }
                set { data[_s2vsa] = value; }
            }
            public byte S2vsb
            {
                get { return (byte)data[_s2vsb]; }
                set { data[_s2vsb] = value; }
            }
            public byte Ola
            {
                get { return (byte)data[_ola]; }
                set { data[_ola] = value; }
            }
            public byte Olb
            {
                get { return (byte)data[_olb]; }
                set { data[_olb] = value; }
            }
            public byte T120
            {
                get { return (byte)data[_t120]; }
                set { data[_t120] = value; }
            }
            public byte T143
            {
                get { return (byte)data[_t143]; }
                set { data[_t143] = value; }
            }
            public byte T150
            {
                get { return (byte)data[_t150]; }
                set { data[_t150] = value; }
            }
            public byte T157
            {
                get { return (byte)data[_t157]; }
                set { data[_t157] = value; }
            }
            public byte CsActual
            {
                get { return (byte)data[_cs_actual]; }
                set { data[_cs_actual] = value; }
            }
            public byte Stealth
            {
                get { return (byte)data[_stealth]; }
                set { data[_stealth] = value; }
            }
            public byte Stst
            {
                get { return (byte)data[_stst]; }
                set { data[_stst] = value; }
            }
        }

        public struct PWMCONF
        {
            public const bool Readable = true;
            public const bool Writable = true;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public PWMCONF()
            {
                data = new BitVector32(0);
            }

            public PWMCONF(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public PWMCONF(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _pwm_ofs = BitVector32.CreateSection(0xff);
            internal static readonly BitVector32.Section _pwm_grad = BitVector32.CreateSection(0xff, _pwm_ofs);
            internal static readonly BitVector32.Section _pwm_freq = BitVector32.CreateSection(0x03, _pwm_grad);
            internal static readonly BitVector32.Section _pwm_autoscale = BitVector32.CreateSection(0x01, _pwm_freq);
            internal static readonly BitVector32.Section _pwm_autograd = BitVector32.CreateSection(0x01, _pwm_autoscale);
            internal static readonly BitVector32.Section _freewheel = BitVector32.CreateSection(0x03, _pwm_autograd);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0x01, _freewheel);
            internal static readonly BitVector32.Section _reserved1 = BitVector32.CreateSection(0x01, _reserved0);
            internal static readonly BitVector32.Section _pwm_reg = BitVector32.CreateSection(0x0f, _reserved1);
            internal static readonly BitVector32.Section _pwm_lim = BitVector32.CreateSection(0x0f, _pwm_reg);

            public Int16 PwmOfs
            {
                get { return (byte)data[_pwm_ofs]; }
                set { data[_pwm_ofs] = value; }
            }
            public Int16 PwmGrad
            {
                get { return (byte)data[_pwm_grad]; }
                set { data[_pwm_grad] = value; }
            }
            public byte PwmFreq
            {
                get { return (byte)data[_pwm_freq]; }
                set { data[_pwm_freq] = value; }
            }
            public byte PwmAutoscale
            {
                get { return (byte)data[_pwm_autoscale]; }
                set { data[_pwm_autoscale] = value; }
            }
            public byte PwmAutograd
            {
                get { return (byte)data[_pwm_autograd]; }
                set { data[_pwm_autograd] = value; }
            }
            public byte Freewheel
            {
                get { return (byte)data[_freewheel]; }
                set { data[_freewheel] = value; }
            }
            public byte PwmReg
            {
                get { return (byte)data[_pwm_reg]; }
                set { data[_pwm_reg] = value; }
            }
            public byte PwmLim
            {
                get { return (byte)data[_pwm_lim]; }
                set { data[_pwm_lim] = value; }
            }
        }

        public struct PWM_SCALE
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public PWM_SCALE()
            {
                data = new BitVector32(0);
            }

            public PWM_SCALE(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public PWM_SCALE(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _pwm_scale_sum = BitVector32.CreateSection(0xff);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0xff, _pwm_scale_sum);
            internal static readonly BitVector32.Section _pwm_scale_auto = BitVector32.CreateSection(0x01ff, _reserved0);

            public byte PwmScaleSum
            {
                get { return (byte)data[_pwm_scale_sum]; }
                set { data[_pwm_scale_sum] = value; }
            }
            public Int16 PwmScaleAuto
            {
                get { return (Int16)data[_pwm_scale_auto]; }
                set { data[_pwm_scale_auto] = value; }
            }
        }

        public struct PWM_AUTO
        {
            public const bool Readable = true;
            public const bool Writable = false;

            internal BitVector32 data;

            public int Val { get { return data.Data; } }

            public PWM_AUTO()
            {
                data = new BitVector32(0);
            }

            public PWM_AUTO(BitVector32 v)
            {
                data = new BitVector32(v);
            }

            public PWM_AUTO(int v)
            {
                data = new BitVector32(v);
            }

            internal static readonly BitVector32.Section _pwm_ofs_auto = BitVector32.CreateSection(0xff);
            internal static readonly BitVector32.Section _reserved0 = BitVector32.CreateSection(0xff, _pwm_ofs_auto);
            internal static readonly BitVector32.Section _pwm_grad_auto = BitVector32.CreateSection(0xff, _reserved0);

            public byte PwmOfsAuto
            {
                get { return (byte)data[_pwm_ofs_auto]; }
                set { data[_pwm_ofs_auto] = value; }
            }
            public byte PwmGradAuto
            {
                get { return (byte)data[_pwm_grad_auto]; }
                set { data[_pwm_grad_auto] = value; }
            }
        }
    }
}
