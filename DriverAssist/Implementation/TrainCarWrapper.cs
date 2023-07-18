using DV.HUD;
using DV.Simulation.Cars;
using LocoSim.Implementations;

namespace DriverAssist.Implementation
{
    internal class DVTrainCarWrapper : TrainCarWrapper
    {
        private TrainCar loco;
        private BaseControlsOverrider obj;
        private BaseControlsOverrider baseControlsOverrider;
        private LocoIndicatorReader locoIndicatorReader;
        private SimulationFlow simFlow;
        private Port torqueGeneratedPort;

        public DVTrainCarWrapper(TrainCar car)
        {
            this.loco = car;
            this.baseControlsOverrider = loco.GetComponent<SimController>()?.controlsOverrider;
            this.obj = loco.GetComponent<SimController>()?.controlsOverrider;
            this.locoIndicatorReader = loco.loadedInterior?.GetComponent<LocoIndicatorReader>();
            this.simFlow = loco.GetComponent<SimController>()?.simFlow;
            string torqueGeneratedPortId = loco.GetComponent<SimController>()?.drivingForce.torqueGeneratedPortId;
            simFlow.TryGetPort(torqueGeneratedPortId, out torqueGeneratedPort);
        }

        public bool IsLoco { get { return true; } }

        public float SpeedKmh
        {
            get
            {
                float speed = loco.GetForwardSpeed() * 3.6f;
                return speed;
            }
        }

        public float SpeedMs
        {
            get
            {
                float speed = loco.GetForwardSpeed();
                return speed;
            }
        }

        public float Throttle
        {
            get
            {
                return obj.Throttle.Value;
            }
            set
            {
                obj.Throttle?.Set(value);
            }
        }

        public float TrainBrake
        {
            get
            {
                return obj.Brake.Value;
            }
            set
            {
                obj.Brake.Set(value);
            }
        }

        public float IndBrake
        {
            get
            {
                return obj.IndependentBrake.Value;
            }
            set
            {
                obj.IndependentBrake.Set(value);
            }
        }

        public float GearboxA
        {
            get
            {
                BaseControlsOverrider overrider = loco.GetComponentInChildren<BaseControlsOverrider>(includeInactive: true);
                InteriorControlsManager controls = loco.interior.GetComponentInChildren<InteriorControlsManager>();
                if (!controls.TryGetControl(InteriorControlsManager.ControlType.GearboxA, out var reference))
                {
                    return 0;
                }
                return reference.controlImplBase.Value;
            }
            // return overrider.bas;

            set
            {
                BaseControlsOverrider overrider = loco.GetComponentInChildren<BaseControlsOverrider>(includeInactive: true);
                InteriorControlsManager controls = loco.interior.GetComponentInChildren<InteriorControlsManager>();
                if (controls.TryGetControl(InteriorControlsManager.ControlType.GearboxA, out var reference))
                {
                    // controls.TryGetControl(InteriorControlsManager.ControlType.GearboxA, out var reference6)
                    reference.controlImplBase.SetValue(value);
                }
                // return reference.controlImplBase.Value;
            }
        }

        public float GearboxB
        {
            get
            {
                BaseControlsOverrider overrider = loco.GetComponentInChildren<BaseControlsOverrider>(includeInactive: true);
                InteriorControlsManager controls = loco.interior.GetComponentInChildren<InteriorControlsManager>();
                if (!controls.TryGetControl(InteriorControlsManager.ControlType.GearboxB, out var reference))
                {
                    return 0;
                }
                return reference.controlImplBase.Value;
            }
            // return overrider.bas;

            set
            {
                BaseControlsOverrider overrider = loco.GetComponentInChildren<BaseControlsOverrider>(includeInactive: true);
                InteriorControlsManager controls = loco.interior.GetComponentInChildren<InteriorControlsManager>();
                if (controls.TryGetControl(InteriorControlsManager.ControlType.GearboxB, out var reference))
                {
                    // controls.TryGetControl(InteriorControlsManager.ControlType.GearboxA, out var reference6)
                    reference.controlImplBase.SetValue(value);
                }
                // return reference.controlImplBase.Value;
            }
        }

        public float Temperature
        {
            get
            {
                if (!locoIndicatorReader)
                {
                    return 0;
                }

                if (locoIndicatorReader?.tmTemp)
                    return locoIndicatorReader?.tmTemp?.Value ?? 0;
                if (locoIndicatorReader?.oilTemp)
                    return locoIndicatorReader?.oilTemp?.Value ?? 0;

                return 0;
            }
        }

        public float Reverser
        {
            get
            {
                return obj.Reverser.Value;
            }

            set
            {
                obj.Reverser.Set(value);
            }
        }


        public float Torque
        {
            get
            {
                return torqueGeneratedPort.Value;
            }
        }

        public bool IsElectric
        {
            get
            {
                switch (LocoType)
                {
                    case DriverAssist.LocoType.DE2:
                    case DriverAssist.LocoType.DE6:
                        return true;
                    default:
                        return false;
                }
            }

            // simFlow.TryGetPort("tm.WORKING_TRACTION_MOTORS", out port);

            // foreach (SimComponent x in simFlow.orderedSimComps)
            // {
            //     List<Port> ports = x.GetAllPorts();
            //     foreach (Port p in ports)
            //     {
            //         PluginLoggerSingleton.Instance.Info($"{x} {p.id}");
            //     }
            // }
            // string torqueGeneratedPortId = locoCar.GetComponent<SimController>()?.tr;

            // locoCar.def
            // return "" + port.Value;
            // return torqueGeneratedPortId;
            // }
        }

        public string TractionMotors
        {
            get
            {
                // int x = locoCar.GetComponent<TractionMotor>().numberOfTractionMotors;
                // SimulationFlow simFlow = loco.GetComponent<SimController>()?.simFlow;
                Port port;
                string maxAmps = "";
                string motors = "";
                // string torqueGeneratedPortId = locoCar.GetComponent<SimController>()?.drivingForce.torqueGeneratedPortId;
                if (IsElectric)
                {
                    if (simFlow.TryGetPort("tm.MAX_AMPS", out port))
                        maxAmps = "" + port.Value;
                    if (simFlow.TryGetPort("tm.WORKING_TRACTION_MOTORS", out port))
                        motors = "" + port.Value;
                    return $"{motors} / {maxAmps}";
                }
                else { return ""; }
            }

            // simFlow.TryGetPort("tm.WORKING_TRACTION_MOTORS", out port);

            // foreach (SimComponent x in simFlow.orderedSimComps)
            // {
            //     List<Port> ports = x.GetAllPorts();
            //     foreach (Port p in ports)
            //     {
            //         PluginLoggerSingleton.Instance.Info($"{x} {p.id}");
            //     }
            // }
            // string torqueGeneratedPortId = locoCar.GetComponent<SimController>()?.tr;

            // locoCar.def
            // return "" + port.Value;
            // return torqueGeneratedPortId;
            // }
        }

        public float Amps
        {
            get
            {
                return locoIndicatorReader?.amps?.Value ?? 0;
            }
        }

        public float Rpm
        {
            get
            {
                return locoIndicatorReader?.engineRpm?.Value ?? 0;
            }
        }

        public string LocoType
        {
            get
            {
                return loco.carType.ToString();
            }
        }

        public float Mass
        {
            get
            {
                float mass = 0;

                foreach (TrainCar car in loco.trainset.cars)
                {
                    mass += car.massController.TotalMass;
                }

                return mass;
            }
        }

        public float LocoMass
        {
            get
            {
                float mass = 0;

                foreach (TrainCar car in loco.trainset.cars)
                {
                    if (car.IsLoco) mass += car.massController.TotalMass;
                }

                return mass;
            }
        }

        public float CargoMass
        {
            get
            {
                float mass = 0;

                foreach (TrainCar car in loco.trainset.cars)
                {
                    if (!car.IsLoco) mass += car.massController.TotalMass;
                }

                return mass;
            }
        }
    }
}
