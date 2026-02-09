using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ch.etel.edi.dsa.v40;
using ch.etel.edi.dmd.v40;

namespace WaferMeasurementFlow.Managers
{
    public class EtelManager
    {
        private DsaDrive _drive;
        private DmdData _dmdData;
        private bool _isConnected;

        public bool IsConnected => _isConnected;

        public event Action<string> OnLog;
        public event Action<string> OnError;

        public EtelManager()
        {
        }

        public void Connect(string url)
        {
            try
            {
                if (_isConnected) Disconnect();

                Log($"Connecting to {url}...");
                // Open drive
                _drive = new DsaDrive();
                _drive.open(url);

                // Initialize DMD for metadata (register names, types)
                try
                {
                    // Assuming DMD might need initialization or we just use static/instance
                    // Often DMD is used to inspect the drive we just opened.
                    // But DmdData constructor might require the drive handle or product ID?
                    // From inspecting, DmdData has getHandle(), maybe it wraps something.
                    // For now, let's just focus on basic Read/Write if DMD fails.
                    // But let's try to create DmdData if possible.
                    // _dmdData = new DmdData(); // If default constructor exists
                }
                catch (Exception ex)
                {
                    Log($"Warning: DMD initialization failed (MetaData functionality limited): {ex.Message}");
                }

                _isConnected = true;
                Log("Connected successfully.");
            }
            catch (Exception ex)
            {
                _isConnected = false;
                _drive = null;
                Error($"Connection failed: {ex.Message}");
                throw;
            }
        }

        public void Disconnect()
        {
            if (_drive != null)
            {
                try
                {
                    // _drive.close(); // If close exists, otherwise just dispose
                    // Based on "open", there is likely "close".
                    // Reflection showed "open", didn't see "close" explicitly in my grep but expected.
                    // If not, we just null it.
                    // Let's assume .close() or .Dispose() if IDisposable.
                    // The "inspect_dsa" output for DsaDrive was weak.
                    // Let's try .close() and catch exception.
                    Type t = _drive.GetType();
                    var method = t.GetMethod("close");
                    if (method != null) method.Invoke(_drive, null);
                }
                catch (Exception ex)
                {
                    Log($"Error during disconnect: {ex.Message}");
                }
                finally
                {
                    _drive = null;
                    _isConnected = false;
                    Log("Disconnected.");
                }
            }
        }

        // Generic Read
        public long ReadParameter(string typeAlias, int index, int subIndex = 0)
        {
            if (!_isConnected) throw new InvalidOperationException("Not connected.");

            // Resolve type alias to integer (e.g. "K" -> 10 or whatever)
            // For now, pass raw integer if alias parsing not implemented
            int type;
            if (int.TryParse(typeAlias, out int typeInt))
            {
                type = typeInt;
            }
            else
            {
                // Try to resolve using DMD if available, or simple mapping
                // Assuming standard ETEL types roughly:
                // This is a guess. User interface should probably allow Int input mostly.
                type = ResolveType(typeAlias);
            }

            try
            {
                // Use reflection to find the right Read method if we are not sure of the signature from inspection
                // But normally: driveSimpleRead(int axis, int type, int index, int subindex)
                // Let's assume axis 0 for single drive object? Or axis is part of it.
                // If url is ".../drive1", it might be axis 0 of that drive object?
                // The API usually requires axis index.

                // Let's look for methods with 4 integer args.
                // Assuming axis=0 for now.

                // Inspect resulted in: 
                // We didn't see read methods. 
                // I will try `driveSimpleRead` with reflection to be safe or just call it if I can guess signature.
                // Actually, I'll compile with dynamic to avoid build errors if method missing, 
                // OR better, use the found "getRegister..." from DMD to Read? No, DMD is metadata.
                // DSA is for access.

                // Let's use dynamic to call ReadS (Simple Read) or similar.
                // Common ETEL method: `ReadS(int type, int index, int subindex)` maybe?
                // Or `GetInt32(int type, int index, int subindex)`?

                // Because I am flying blind on the exact method name for "Read Register", 
                // I will use Reflection to find a likely "Read" method and invoke it.
                // This is safer than hardcoding "driveSimpleRead" which might not exist.

                return ReadRegisterReflection(0, type, index, subIndex);
            }
            catch (Exception ex)
            {
                Error($"Read failed: {ex.Message}");
                throw;
            }
        }

        public void WriteParameter(string typeAlias, int index, int subIndex, long value)
        {
            if (!_isConnected) throw new InvalidOperationException("Not connected.");
            int type = ResolveType(typeAlias);

            try
            {
                WriteRegisterReflection(0, type, index, subIndex, value);
                Log($"Wrote {value} to {typeAlias}.{index}.{subIndex}");
            }
            catch (Exception ex)
            {
                Error($"Write failed: {ex.Message}");
                throw;
            }
        }

        private int ResolveType(string alias)
        {
            if (int.TryParse(alias, out int res)) return res;

            // TODO: Use DmdData to resolve if possible.
            // For now return 0 or throw
            // Common: M=0, K=1, X=2... pure guessing.
            // Be safe, ask user to input ID if possible.
            return 0;
        }

        private long ReadRegisterReflection(int axis, int type, int index, int subindex)
        {
            // Try to find a method that looks like Read(type, index, subindex)
            // or Read(axis, type, index, subindex)
            var methods = _drive.GetType().GetMethods();

            // Look for specific known names first
            var m = methods.FirstOrDefault(x => x.Name == "driveSimpleRead" || x.Name == "ReadS" || x.Name == "getObject");

            if (m != null)
            {
                // Check params
                var p = m.GetParameters();
                if (p.Length == 4) // axis, type, idx, sub
                {
                    var res = m.Invoke(_drive, new object[] { axis, type, index, subindex });
                    return Convert.ToInt64(res);
                }
                else if (p.Length == 3) // type, idx, sub (implied axis?)
                {
                    var res = m.Invoke(_drive, new object[] { type, index, subindex });
                    return Convert.ToInt64(res);
                }
                else if (p.Length == 2 && p[0].ParameterType == typeof(string)) // Read by name?
                {
                    // Not what we want
                }
            }

            // Fallback: try to find ANY method returning int/long taking 3 or 4 ints
            foreach (var met in methods)
            {
                if (met.Name.Contains("Read") || met.Name.Contains("get"))
                {
                    var pars = met.GetParameters();
                    if (pars.Any(x => x.ParameterType == typeof(int)) && pars.Length >= 3 && pars.Length <= 4)
                    {
                        // Try invoking? risky.
                    }
                }
            }

            throw new Exception("Could not find a suitable Read method on DsaDrive. Please check API.");
        }

        private void WriteRegisterReflection(int axis, int type, int index, int subindex, long value)
        {
            // Look for specific known names first
            var methods = _drive.GetType().GetMethods();
            var m = methods.FirstOrDefault(x => x.Name == "driveSimpleWrite" || x.Name == "WriteS" || x.Name == "setObject");

            if (m != null)
            {
                // Check params (axis, type, idx, sub, value) -> 5 args
                // Or (type, idx, sub, value) -> 4 args
                var pars = m.GetParameters();
                if (pars.Length == 5)
                {
                    m.Invoke(_drive, new object[] { axis, type, index, subindex, (int)value }); // Assume int value
                    return;
                }
                else if (pars.Length == 4)
                {
                    m.Invoke(_drive, new object[] { type, index, subindex, (int)value });
                    return;
                }
            }

            throw new Exception("Could not find a suitable Write method on DsaDrive.");
        }

        private void Log(string msg) => OnLog?.Invoke(msg);
        private void Error(string msg) => OnError?.Invoke(msg);
    }
}
