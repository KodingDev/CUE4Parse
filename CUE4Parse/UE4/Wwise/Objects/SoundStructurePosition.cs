using System.Text.Json;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects
{
    public class SoundStructurePosition
    {
        public readonly bool PositionIncluded;
        public readonly EPositionDimensionType? PositionDimension;
        public readonly bool? Position2DPanner;
        public readonly EPosition3DSource? Position3DSource;
        public readonly uint? Position3DAttenuationId;
        public readonly bool? Position3DSpatialization;
        public readonly EPosition3DPlayType? Position3DPlayType;
        public readonly bool? Position3DLoop;
        public readonly uint? Position3DTransitionTime;
        public readonly bool? Position3DFollowListenerOrientation;
        public readonly bool? Position3DUpdatePerFrame;

        public SoundStructurePosition(FArchive Ar)
        {
            PositionIncluded = Ar.Read<bool>();
            if (PositionIncluded)
            {
                PositionDimension = Ar.Read<EPositionDimensionType>();
                if (PositionDimension == EPositionDimensionType.TwoD)
                    Position2DPanner = Ar.Read<bool>();
                else if (PositionDimension == EPositionDimensionType.ThreeD)
                {
                    Position3DSource = Ar.Read<EPosition3DSource>();
                    Position3DAttenuationId = Ar.Read<uint>();
                    Position3DSpatialization = Ar.Read<bool>();
                    if (Position3DSource == EPosition3DSource.UserDefined)
                    {
                        Position3DPlayType = Ar.Read<EPosition3DPlayType>();
                        Position3DLoop = Ar.Read<bool>();
                        Position3DTransitionTime = Ar.Read<uint>();
                        Position3DFollowListenerOrientation = Ar.Read<bool>();
                    }
                    else if (Position3DSource == EPosition3DSource.GameDefined)
                        Position3DUpdatePerFrame = Ar.Read<bool>();
                }
            }
        }

        public void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("PositionIncluded");
            writer.WriteBooleanValue(PositionIncluded);

            if (PositionIncluded)
            {
                writer.WritePropertyName("PositionDimension");
                JsonSerializer.Serialize(writer, PositionDimension, options);

                if (PositionDimension == EPositionDimensionType.TwoD)
                {
                    if (Position2DPanner.HasValue)
                    {
                        writer.WritePropertyName("Position2DPanner");
                        writer.WriteBooleanValue(Position2DPanner.Value);
                    }
                }
                else if (PositionDimension == EPositionDimensionType.ThreeD)
                {
                    if (Position3DSource.HasValue)
                    {
                        writer.WritePropertyName("Position3DSource");
                        JsonSerializer.Serialize(writer, Position3DSource.Value, options);
                    }

                    if (Position3DAttenuationId.HasValue)
                    {
                        writer.WritePropertyName("Position3DAttenuationId");
                        writer.WriteNumberValue(Position3DAttenuationId.Value);
                    }

                    if (Position3DSpatialization.HasValue)
                    {
                        writer.WritePropertyName("Position3DSpatialization");
                        writer.WriteBooleanValue(Position3DSpatialization.Value);
                    }

                    if (Position3DSource == EPosition3DSource.UserDefined)
                    {
                        if (Position3DPlayType.HasValue)
                        {
                            writer.WritePropertyName("Position3DPlayType");
                            JsonSerializer.Serialize(writer, Position3DPlayType.Value, options);
                        }

                        if (Position3DLoop.HasValue)
                        {
                            writer.WritePropertyName("Position3DLoop");
                            writer.WriteBooleanValue(Position3DLoop.Value);
                        }

                        if (Position3DTransitionTime.HasValue)
                        {
                            writer.WritePropertyName("Position3DTransitionTime");
                            writer.WriteNumberValue(Position3DTransitionTime.Value);
                        }

                        if (Position3DFollowListenerOrientation.HasValue)
                        {
                            writer.WritePropertyName("Position3DFollowListenerOrientation");
                            writer.WriteBooleanValue(Position3DFollowListenerOrientation.Value);
                        }
                    }
                    else if (Position3DSource == EPosition3DSource.GameDefined)
                    {
                        if (Position3DUpdatePerFrame.HasValue)
                        {
                            writer.WritePropertyName("Position3DUpdatePerFrame");
                            writer.WriteBooleanValue(Position3DUpdatePerFrame.Value);
                        }
                    }
                }
            }

            writer.WriteEndObject();
        }
    }
}
