using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Beamable.Samples.Core.UI
{
    /// <summary>
    /// User may click any <link=blah>text</link> in the source text to
    /// open a url in the default system web browser.
    /// 
    /// <see cref="https://deltadreamgames.com/unity-tmp-hyperlinks/"/>
    /// </summary>
    public class TMPHyperlinkHandler : MonoBehaviour, IPointerClickHandler
    {
        //  Fields ---------------------------------------
        [SerializeField]
        private TextMeshProUGUI _textMesh = null;

        //  Unity Methods -------------------------------
        protected void Start()
        {
            Assert.IsNotNull(_textMesh);
        }

        //  Event Handlers -------------------------------
        public void OnPointerClick(PointerEventData eventData)
        {
            // For certain situations (Unity Version?, Canvas Mode?), toggle this per project
            bool isConvertToWorld = false;
          
            int linkIndex = FindIntersectingLink(_textMesh, Input.mousePosition, Camera.main, isConvertToWorld);
         
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = _textMesh.textInfo.linkInfo[linkIndex];
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
      
      
        /// <summary>
        /// Function returning the index of the Link at the given position (if any).
        /// </summary>
        /// <param name="text">A reference to the TMP_Text component.</param>
        /// <param name="position">Position to check for intersection.</param>
        /// <param name="camera">The scene camera which may be assigned to a Canvas using ScreenSpace Camera or WorldSpace render mode. Set to null is using ScreenSpace Overlay.</param>
        /// <param name="isConvertToWorld"></param>
        /// <returns></returns>
        public static int FindIntersectingLink(TMP_Text text, Vector3 position, Camera camera, bool isConvertToWorld)
        { 
            Transform rectTransform = text.transform;

            if (isConvertToWorld)
            {
                // Convert position into Worldspace coordinates
                TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
            }

            //Debug.Log("position is  [" + position+ "]");
            
            for (int i = 0; i < text.textInfo.linkCount; i++)
            {
                TMP_LinkInfo linkInfo = text.textInfo.linkInfo[i];
                //Debug.Log("linkInfo [" + linkInfo + "]");
                
                bool isBeginRegion = false;

                Vector3 bl = Vector3.zero;
                Vector3 tl = Vector3.zero;
                Vector3 br = Vector3.zero;
                Vector3 tr = Vector3.zero;

                // Iterate through each character of the word
                for (int j = 0; j < linkInfo.linkTextLength; j++)
                {
                    int characterIndex = linkInfo.linkTextfirstCharacterIndex + j;
                    TMP_CharacterInfo currentCharInfo = text.textInfo.characterInfo[characterIndex];
                    int currentLine = currentCharInfo.lineNumber;

                    // Check if Link characters are on the current page
                    if (text.overflowMode == TextOverflowModes.Page && currentCharInfo.pageNumber + 1 != text.pageToDisplay) continue;

                    if (isBeginRegion == false)
                    {
                        isBeginRegion = true;

                        bl = rectTransform.TransformPoint(new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.descender, 0));
                        tl = rectTransform.TransformPoint(new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.ascender, 0));

                        //Debug.Log("Start Word Region at [" + currentCharInfo.character + "]");

                        // If Word is one character
                        if (linkInfo.linkTextLength == 1)
                        {
                            isBeginRegion = false;

                            br = rectTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, currentCharInfo.descender, 0));
                            tr = rectTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, currentCharInfo.ascender, 0));

                            // Check for Intersection
                            if (PointIntersectRectangle(position, bl, tl, tr, br))
                                return i;

                            //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                        }
                    }

                    // Last Character of Word
                    if (isBeginRegion && j == linkInfo.linkTextLength - 1)
                    {
                        isBeginRegion = false;

                        br = rectTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, currentCharInfo.descender, 0));
                        tr = rectTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, currentCharInfo.ascender, 0));

                        // Check for Intersection
                        if (PointIntersectRectangle(position, bl, tl, tr, br))
                            return i;

                        //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                    }
                    // If Word is split on more than one line.
                    else if (isBeginRegion && currentLine != text.textInfo.characterInfo[characterIndex + 1].lineNumber)
                    {
                        isBeginRegion = false;

                        br = rectTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, currentCharInfo.descender, 0));
                        tr = rectTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, currentCharInfo.ascender, 0));

                        // Check for Intersection
                        if (PointIntersectRectangle(position, bl, tl, tr, br))
                            return i;

                        //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                    }
                }

                //Debug.Log("Word at Index: " + i + " is located at (" + bl + ", " + tl + ", " + tr + ", " + br + ").");

            }

            return -1;
        }

        /// <summary>
        /// Function to check if a Point is contained within a Rectangle.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool PointIntersectRectangle(Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 ab = b - a;
            Vector3 am = m - a;
            Vector3 bc = c - b;
            Vector3 bm = m - b;

            float abamDot = Vector3.Dot(ab, am);
            float bcbmDot = Vector3.Dot(bc, bm);

            return 0 <= abamDot && abamDot <= Vector3.Dot(ab, ab) && 0 <= bcbmDot && bcbmDot <= Vector3.Dot(bc, bc);
        }
    }
}