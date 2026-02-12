using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    // Delegates
    public delegate void FaceLandmarkDataDelegate(List<FaceLandmarkData> faceLandmarkData);

    public class FaceLandmarkData
    {
        public enum FaceLandmarkType
        {
            None = 0,
            PC_6 = 1,
            PC_17 = 2,
            Mobile_17 = 3,
            PC_68 = 4,
            Mobile_68 = 5
        }

        //file:///D:/Git/kweek/Kweek_Unity/Assets/Plugins/DlibFaceLandmarkDetector/ReadMe.pdf
        public enum FaceLandmarkPosition6
        {
            NoseTip = 0,
            NoseBottom = 1,

            LeftEyeLeftSide = 5, 
            LeftEyeRightSide = 4,

            RightEyeLeftSide = 3,
            RightEyeRightSide = 2,
        }

        public enum FaceLandmarkPosition17
        {
            NoseTip = 0,
            NoseBottom = 1,

            LeftEyeLeftSide = 5, 
            LeftEyeRightSide = 4,
            LeftEyeTop = 11,
            LeftEyeBottom = 12,

            RightEyeLeftSide = 3,
            RightEyeRightSide = 2,
            RightEyeTop = 9,
            RightEyeBottom = 10,

            ChinLeft = 8,
            ChinBottom = 7,
            ChinRight = 6,

            MouthLeft = 15,
            MouthRight = 13,
            MountTop = 14,
            MouthBottom = 16,
        }

        public enum FaceLandmarkPosition68
        {
            NoseTip = 30,
            NoseBottom = 33,
            NoseBridge = 27,
            NoseBridge1 = 28,
            NoseBridge2 = 29,        
            NoseLeftSide = 35,
            NoseBottomLeftSide = 34,
            NoseRightSide = 31,
            NoseBottomRightSide = 32,

            LeftEyeLeftSide = 45,
            LeftEyeRightSide = 42,
            LeftEyeTopLeft = 44,
            LeftEyeTopRight = 43,
            LeftEyeBottomLeft = 46,
            LeftEyeBottomRight = 47,

            RightEyeLeftSide = 39,
            RightEyeRightSide = 36,
            RightEyeTopLeft = 38,
            RightEyeTopRight = 37,
            RightEyeBottomLeft = 40,
            RightEyeBottomRight = 41,

            ChinLeftSide = 16,
            ChinLeftSide1 = 15,
            ChinLeftSide2 = 14,
            ChinLeftSide3 = 13,
            ChinLeftSide4 = 12,
            ChinLeftSide5 = 11,
            ChinLeftSide6 = 10,
            ChinLeftSide7 = 9,
            ChinBottom = 8,
            ChinRightSide = 0,
            ChinRightSide1 = 1,
            ChinRightSide2 = 2,
            ChinRightSide3 = 3,
            ChinRightSide4 = 4,
            ChinRightSide5 = 5,
            ChinRightSide6 = 6,
            ChinRightSide7 = 7,

            MouthLeft = 64,
            MouthRight = 60,
            MouthTop = 62,
            MouthBottom = 66,
            MouthTopLeft = 63,
            MouthTopRight = 61,
            MouthBottomLeft = 65,
            MouthBottomRight = 67,

            MouthLeftOutside = 54,
            MouthRightOutside = 48,
            MouthTopOutside = 51,
            MouthBottomOutside = 57,
            MouthTopLeftOutside = 53,
            MouthTopLeft1Outside = 52,
            MouthTopRightOutside = 49,
            MouthTopRight1Outside = 50,
            MouthBottomLeftOutside = 55,
            MouthBottomLeft1Outside = 56,
            MouthBottomRightOutside = 59,
            MouthBottomRight1Outside = 58,

            LeftEyebrowLeftSide = 26,
            LeftEyebromRightSide = 22,
            LeftEyebrowTop = 24,
            LeftEyebrowTopLeft = 25,
            LeftEyebrowTopRight = 23,

            RightEyebrowLeftSide = 21,
            RightEyebromRightSide = 17,
            RightEyebrowTop = 19,
            RightEyebrowTopLeft = 20,
            RightEyebrowTopRight = 18,
        }

        //A generic one for the most important positions
        public enum FaceLandmarkPosition
        {
            NoseTip = 0,
            NoseBottom = 1,

            LeftEye = 2,
            LeftEyeLeftSide = 3,
            LeftEyeRightSide = 4,
            LeftEyeTop = 5,
            LeftEyeBottom = 6,

            RightEye = 7,
            RightEyeLeftSide = 8,
            RightEyeRightSide = 9,
            RightEyeTop = 10,
            RightEyeBottom = 11,
        }

        public enum FaceLandmarkHorizonalPositionInterpoliationType
        {
            None = 0,
            Average = 1,
            Left = 2,
            Right = 3
        }

        public enum FaceLandmarkVerticalPositionInterpoliationType
        {
            None = 0,
            Average = 1,
            Top = 2,
            Bottom = 3
        }

        private FaceLandmarkType m_FaceLandmarkType = FaceLandmarkType.None; //Determines how to interpret the positions
        public FaceLandmarkType FaceLandmarkDataType
        {
            get { return m_FaceLandmarkType; }
        }

        private Rect m_FaceRectangle = Rect.zero;
        public Rect FaceRectangle
        {
            get { return m_FaceRectangle; }
        }

        private List<Vector2> m_FaceLandmarkPositions = null;
        public List<Vector2> FaceLandmarkPositions
        {
            get { return m_FaceLandmarkPositions; }
        }

        public FaceLandmarkData()
        {
            m_FaceLandmarkType = FaceLandmarkType.None;
            m_FaceRectangle = Rect.zero;
            m_FaceLandmarkPositions = null;
        }

        public FaceLandmarkData(FaceLandmarkType faceLandmarkType)
        {
            m_FaceLandmarkType = faceLandmarkType;
            m_FaceRectangle = Rect.zero;
            m_FaceLandmarkPositions = null;
        }

        public FaceLandmarkData(FaceLandmarkType faceLandmarkType, Rect faceRectangle)
        {
            m_FaceLandmarkType = faceLandmarkType;
            m_FaceRectangle = faceRectangle;
            m_FaceLandmarkPositions = null;
        }

        public FaceLandmarkData(FaceLandmarkType faceLandmarkType, Rect faceRectangle, List<Vector2> faceLandmarkPositions)
        {
            m_FaceLandmarkType = faceLandmarkType;
            m_FaceRectangle = faceRectangle;
            m_FaceLandmarkPositions = new List<Vector2>(faceLandmarkPositions);
        }

        public void SetFacelandmarkType(FaceLandmarkType faceLandmarkType)
        {
            m_FaceLandmarkType = faceLandmarkType;
        }

        public void SetFaceRectangle(Rect faceRectangle)
        {
            m_FaceRectangle = faceRectangle;
        }

        public void AddFaceLandmarkPosition(Vector2 faceLandmarkPosition)
        {
            if (m_FaceLandmarkPositions == null)
                m_FaceLandmarkPositions = new List<Vector2>();

            m_FaceLandmarkPositions.Add(faceLandmarkPosition);
        }

        public Vector2 GetLandmarkPosition(FaceLandmarkPosition faceLandmarkPositionType)
        {
            if (m_FaceLandmarkPositions == null || m_FaceLandmarkPositions.Count == 0)
                return Vector2.zero;

            //Find the landmark id depending on the type we're using
            List<int> faceLandmarkIDs = new List<int>();
            FaceLandmarkHorizonalPositionInterpoliationType horizontalInterpolationType = FaceLandmarkHorizonalPositionInterpoliationType.Average;
            FaceLandmarkVerticalPositionInterpoliationType  verticalInterpolationType   = FaceLandmarkVerticalPositionInterpoliationType.Average;

            switch (faceLandmarkPositionType)
            {
                //Nose
                case FaceLandmarkPosition.NoseTip:
                {
                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:      { faceLandmarkIDs.Add((int)FaceLandmarkPosition6.NoseTip);  break; }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.NoseTip); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68: { faceLandmarkIDs.Add((int)FaceLandmarkPosition68.NoseTip); break; }

                        case FaceLandmarkType.None:
                        default:                         { break; }
                    }

                    break;
                }

                case FaceLandmarkPosition.NoseBottom:
                {
                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:      { faceLandmarkIDs.Add((int)FaceLandmarkPosition6.NoseBottom);  break; }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.NoseBottom); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68: { faceLandmarkIDs.Add((int)FaceLandmarkPosition68.NoseBottom); break; }

                        case FaceLandmarkType.None:
                        default:                         { break; }
                    }

                    break;
                }

                //Left Eye
                case FaceLandmarkPosition.LeftEye:
                {
                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.LeftEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.LeftEyeRightSide);
                            break;
                        }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition17.LeftEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition17.LeftEyeRightSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition17.LeftEyeTop);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition17.LeftEyeBottom);
                            break;
                        }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeRightSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeTopLeft);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeTopRight);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeBottomLeft);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeBottomRight);
                            break;
                        }

                        case FaceLandmarkType.None:
                        default:
                        {
                            break;
                        }
                    }

                    break;
                }

                case FaceLandmarkPosition.LeftEyeLeftSide:
                {
                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:      { faceLandmarkIDs.Add((int)FaceLandmarkPosition6.LeftEyeLeftSide);  break; }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.LeftEyeLeftSide); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68: { faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeLeftSide); break; }

                        case FaceLandmarkType.None:
                        default:                         { break; }
                    }

                    break;
                }

                case FaceLandmarkPosition.LeftEyeRightSide:
                {
                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:      { faceLandmarkIDs.Add((int)FaceLandmarkPosition6.LeftEyeRightSide);  break; }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.LeftEyeRightSide); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68: { faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeRightSide); break; }

                        case FaceLandmarkType.None:
                        default:                         { break; }
                    }

                    break;
                }

                case FaceLandmarkPosition.LeftEyeTop:
                {
                    verticalInterpolationType = FaceLandmarkVerticalPositionInterpoliationType.Top;

                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.LeftEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.LeftEyeRightSide);
                            break;
                        }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.LeftEyeTop); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeTopLeft);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeTopRight);
                            break;
                        }

                        case FaceLandmarkType.None:
                        default:
                        {
                            break;
                        }
                    }

                    break;
                }

                case FaceLandmarkPosition.LeftEyeBottom:
                {
                    verticalInterpolationType = FaceLandmarkVerticalPositionInterpoliationType.Bottom;

                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.LeftEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.LeftEyeRightSide);
                            break;
                        }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.LeftEyeBottom); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeBottomLeft);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.LeftEyeBottomRight);
                            break;
                        }

                        case FaceLandmarkType.None:
                        default:
                        {
                            break;
                        }
                    }

                    break;
                }

                //Right Eye
                case FaceLandmarkPosition.RightEye:
                {
                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.RightEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.RightEyeRightSide);
                            break;
                        }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition17.RightEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition17.RightEyeRightSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition17.RightEyeTop);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition17.RightEyeBottom);
                            break;
                        }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeRightSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeTopLeft);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeTopRight);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeBottomLeft);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeBottomRight);
                            break;
                        }

                        case FaceLandmarkType.None:
                        default:
                        {
                            break;
                        }
                    }

                    break;
                }

                case FaceLandmarkPosition.RightEyeLeftSide:
                {
                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:      { faceLandmarkIDs.Add((int)FaceLandmarkPosition6.RightEyeLeftSide);  break; }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.RightEyeLeftSide); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68: { faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeLeftSide); break; }

                        case FaceLandmarkType.None:
                        default:                         { break; }
                    }

                    break;
                }

                case FaceLandmarkPosition.RightEyeRightSide:
                {
                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:      { faceLandmarkIDs.Add((int)FaceLandmarkPosition6.RightEyeRightSide);  break; }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.RightEyeRightSide); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68: { faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeRightSide); break; }

                        case FaceLandmarkType.None:
                        default:                         { break; }
                    }

                    break;
                }

                case FaceLandmarkPosition.RightEyeTop:
                {
                    verticalInterpolationType = FaceLandmarkVerticalPositionInterpoliationType.Top;

                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.RightEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.RightEyeRightSide);
                            break;
                        }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.RightEyeTop); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeTopLeft);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeTopRight);
                            break;
                        }

                        case FaceLandmarkType.None:
                        default:
                        {
                            break;
                        }
                    }

                    break;
                }

                case FaceLandmarkPosition.RightEyeBottom:
                {
                    verticalInterpolationType = FaceLandmarkVerticalPositionInterpoliationType.Bottom;

                    switch (m_FaceLandmarkType)
                    {
                        case FaceLandmarkType.PC_6:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.RightEyeLeftSide);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition6.RightEyeRightSide);
                            break;
                        }

                        case FaceLandmarkType.PC_17:
                        case FaceLandmarkType.Mobile_17: { faceLandmarkIDs.Add((int)FaceLandmarkPosition17.RightEyeBottom); break; }

                        case FaceLandmarkType.PC_68:
                        case FaceLandmarkType.Mobile_68:
                        {
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeBottomLeft);
                            faceLandmarkIDs.Add((int)FaceLandmarkPosition68.RightEyeBottomRight);
                            break;
                        }

                        case FaceLandmarkType.None:
                        default:
                        {
                            break;
                        }
                    }

                    break;
                }
            }

            if (faceLandmarkIDs.Count == 0)
                return Vector2.zero;

            //Shortcut
            if (faceLandmarkIDs.Count == 1)
            {
                int faceLandmarkID = faceLandmarkIDs[0];
                if (faceLandmarkID < 0 || faceLandmarkID >= m_FaceLandmarkPositions.Count)
                    return Vector2.zero;

                return m_FaceLandmarkPositions[faceLandmarkID];
            }

            //Gather all the positions
            List<Vector2> faceLandmarkPositions = new List<Vector2>();
            foreach(int faceLandmarkID in faceLandmarkIDs)
            {
                if (faceLandmarkID < 0 || faceLandmarkID >= m_FaceLandmarkPositions.Count)
                    return Vector2.zero;

                faceLandmarkPositions.Add(m_FaceLandmarkPositions[faceLandmarkID]);
            }

            //Interpolate the positions
            Vector2 firstLandmarkPosition = faceLandmarkPositions[0];

            float cummulativePositionX = 0.0f;
            float leftPositionX = firstLandmarkPosition.x;
            float rightPositionX = firstLandmarkPosition.x;

            float cummulativePositionY = 0.0f;
            float topPositionY = firstLandmarkPosition.y;
            float bottomPositionY = firstLandmarkPosition.y;

            foreach (Vector2 faceLandmarkPosition in faceLandmarkPositions)
            {
                cummulativePositionX += faceLandmarkPosition.x;
                cummulativePositionY += faceLandmarkPosition.y;

                if (faceLandmarkPosition.x > leftPositionX) //The face left. Not left on the picture.
                    leftPositionX = faceLandmarkPosition.x;

                if (faceLandmarkPosition.x < rightPositionX) //The face right. Not right on the picture.
                    rightPositionX = faceLandmarkPosition.x;

                if (faceLandmarkPosition.y > topPositionY)
                    topPositionY = faceLandmarkPosition.y;

                if (faceLandmarkPosition.y < bottomPositionY)
                    bottomPositionY = faceLandmarkPosition.y;
            }

            //Combine the interpolation for the result
            Vector2 result = Vector2.zero;
            switch (horizontalInterpolationType)
            {
                case FaceLandmarkHorizonalPositionInterpoliationType.Average: { result.x = cummulativePositionX / (float)faceLandmarkPositions.Count; break; } //Count > 0 at this point
                case FaceLandmarkHorizonalPositionInterpoliationType.Left:    { result.x = leftPositionX;  break; }
                case FaceLandmarkHorizonalPositionInterpoliationType.Right:   { result.x = rightPositionX; break; }

                case FaceLandmarkHorizonalPositionInterpoliationType.None:
                default:                                                      { break; }
            }
            
            switch (verticalInterpolationType)
            {
                case FaceLandmarkVerticalPositionInterpoliationType.Average: { result.y = cummulativePositionY / (float)faceLandmarkPositions.Count; break; } //Count > 0 at this point
                case FaceLandmarkVerticalPositionInterpoliationType.Top:     { result.y = topPositionY;    break; }
                case FaceLandmarkVerticalPositionInterpoliationType.Bottom:  { result.y = bottomPositionY; break; }

                case FaceLandmarkVerticalPositionInterpoliationType.None:
                default:                                                     { break; }
            }

            return result;
        }

        public float CalculateEyeAspectRatio()
        {
            //https://www.geeksforgeeks.org/python/eye-blink-detection-with-opencv-python-and-dlib/

            //Horizontal Distance
            //Vector2 

            //Vertical Distances
            return 0.0f;
        }
    }
}