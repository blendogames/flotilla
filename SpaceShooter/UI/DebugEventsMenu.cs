#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
#endregion

namespace SpaceShooter
{
    public class DebugEventsMenu : SysMenu
    {
        Vector2 evDebugPos = Vector2.Zero;
        Vector2 evDebugRightClickOffset = Vector2.Zero;
        Vector2 evDebugLeftClickOffset = Vector2.Zero;
        List<evDebugNode> evDebugNodes;

        float fontSize = 1;

        public DebugEventsMenu()
        {
            evDebugNodes = new List<evDebugNode>();

            List<string> directories = new List<string>();
            string dir = Directory.GetCurrentDirectory() + "../../../../Events/";
            fileListing(directories, dir);

            foreach (string fullpath in directories)
            {
                string output = "";

                evDebugNode evNode = new evDebugNode();
                evNode.functionNames = new List<string>();
                evNode.unlockLinks = new List<linkString>();
                evNode.keyLinks = new List<linkString>();

                using (FileStream stream = File.Open(fullpath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        //dump file content into a string.
                        output = reader.ReadToEnd();
                    }
                }

                //class name.
                int idx = 0;
                int fIdx = 0;
                string tmp = Helpers.GetSubString(output, "public class", ":", out idx, out fIdx);
                if (tmp != null)
                {
                    evNode.fileName = tmp;
                    output = output.Remove(0, idx);
                }

                ParseStrings(output, evNode, "UnlockEvent(new", "(", linkType.unlockLink);
                ParseStrings(output, evNode, "AddKey(typeof(", ")", linkType.keyLink);
                ParseStrings(output, evNode, "void", "(", linkType.voidLink);

                evDebugNodes.Add(evNode);
            }

            AddLinks();

            UpdateNodePositions();
        }

        public enum linkType
        {
            voidLink,
            unlockLink,
            keyLink,            
        }

        //get the # of the function that precedes the startIndex
        private int GetFuncNum(string output, int startIndex)
        {
            //number of Voids in the file.
            List<int> amountOfVoids = new List<int>();

            int curIndex = 0;
            bool check = true;
            while (check == true)
            {
                int i = output.IndexOf("void", curIndex);

                if (i < 0 || i >= startIndex)
                {
                    check = false;
                    continue;
                }

                curIndex = i+1;
                amountOfVoids.Add(i);
            }

            
            return Math.Max(0, amountOfVoids.Count - 1);
        }

        private void ParseStrings(string output, evDebugNode evNode, string strStart, string strEnd, linkType LinkType)
        {
            bool check = true;
            string tempOutput = output;

            int totalIndex = 0;

            while (check == true)
            {
                int index = 0;
                int fIdx = 0;

                string tempString = Helpers.GetSubString(tempOutput, strStart, strEnd, out index, out fIdx);
                if (tempString != null)
                {
                    if (LinkType == linkType.unlockLink)
                    {
                        linkString link = new linkString();
                        link.linkname = tempString;
                        link.funcNum = GetFuncNum(output, index + totalIndex);
                        evNode.unlockLinks.Add(link);
                    }
                    else if (LinkType == linkType.keyLink)
                    {
                        linkString link = new linkString();
                        link.linkname = tempString;
                        link.funcNum = GetFuncNum(output, index + totalIndex);
                        evNode.keyLinks.Add(link);
                    }
                    else if (LinkType == linkType.voidLink)
                        evNode.functionNames.Add(tempString);

                    totalIndex += index;

                    //chop off all text above the current index position.
                    tempOutput = tempOutput.Remove(0, index);
                    continue;
                }

                check = false;
            }
        }


        private void AddLinks()
        {
            //data is now set. now parse through the data and make the necessary line links.
            foreach (evDebugNode node1 in evDebugNodes)
            {
                if (node1.unlockLinks.Count > 0)
                {
                    foreach (linkString unlockLink in node1.unlockLinks)
                    {
                        foreach (evDebugNode node2 in evDebugNodes)
                        {
                            if (string.Compare(unlockLink.linkname, node2.fileName, true) == 0)
                            {
                                //they match. make a link.
                                unlockLink.linkObject = node2;
                                //node2.pos.Y = node1.pos.Y;
                            }
                        }
                    }
                }


                if (node1.keyLinks.Count > 0)
                {
                    foreach (linkString keyLink in node1.keyLinks)
                    {
                        foreach (evDebugNode node2 in evDebugNodes)
                        {
                            if (string.Compare(keyLink.linkname, node2.fileName, true) == 0)
                            {
                                //they match. make a link.
                                keyLink.linkObject = node2;
                                //node2.pos.Y = node1.pos.Y;
                            }
                        }
                    }
                }
            }
        }

        public class linkString
        {
            public int funcNum;
            public string linkname;
            public evDebugNode linkObject;
        }

        public class evDebugNode
        {
            public Vector2 pos = Vector2.Zero;
            public string fileName;
            public List<string> functionNames = new List<string>();

            public List<linkString> unlockLinks = new List<linkString>();
            public List<linkString> keyLinks = new List<linkString>();
        }

        evDebugNode selectedNode = null;

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (inputManager.mouseWheelDown)
            {
                fontSize = MathHelper.Clamp(fontSize - 0.3f, 0.1f, 2.0f);
            }
            else if (inputManager.mouseWheelUp)
            {
                fontSize = MathHelper.Clamp(fontSize + 0.3f, 0.1f, 2.0f);
            }

            if (inputManager.mouseRightHeld)
            {
                if (inputManager.mouseRightStartHold)
                {
                    evDebugRightClickOffset = inputManager.mousePos - evDebugPos;
                }

                if (inputManager.mouseHasMoved)
                    evDebugPos = inputManager.mousePos - evDebugRightClickOffset;
            }

            if (inputManager.mouseLeftHeld)
            {
                if (inputManager.mouseLeftStartHold)
                {
                    selectedNode = SelectNearestNode(inputManager.mousePos);

                    if (selectedNode != null)
                        evDebugLeftClickOffset = inputManager.mousePos - selectedNode.pos;
                }

                if (inputManager.mouseHasMoved && selectedNode != null)
                {
                    selectedNode.pos = inputManager.mousePos - evDebugLeftClickOffset;
                }
            }
            else
            {
                if (selectedNode != null)
                    selectedNode = null;
            }


            base.Update(gameTime, inputManager);
        }

        private evDebugNode SelectNearestNode(Vector2 mousePos)
        {
            int nearestDist = int.MaxValue;
            evDebugNode nearestNode = null;

            foreach (evDebugNode node in evDebugNodes)
            {
                int curDist = (int)Vector2.Distance(node.pos + evDebugPos, mousePos);
                if (curDist < 192 && curDist < nearestDist)
                {
                    nearestDist = curDist;
                    nearestNode = node;
                }
            }

            return nearestNode;
        }

        private void UpdateNodePositions()
        {
            Vector2 lineVec = FrameworkCore.Serif.MeasureString("Z");
            Vector2 pos = new Vector2(100, 100);
            foreach (evDebugNode node1 in evDebugNodes)
            {
                node1.pos = pos;
                pos.Y += lineVec.Y * (node1.functionNames.Count + 2);
            }

            foreach (evDebugNode node1 in evDebugNodes)
            {
                int unlockOffset = 0;
                foreach (linkString link in node1.unlockLinks)
                {
                    if (link.linkObject == null)
                        continue;

                    link.linkObject.pos.X += FrameworkCore.r.Next(192,384);
                    link.linkObject.pos.Y = node1.pos.Y + unlockOffset;
                    unlockOffset += 96;
                }

                int keyOffset = 0;
                foreach (linkString link in node1.keyLinks)
                {
                    if (link.linkObject == null)
                        continue;

                    link.linkObject.pos.X += FrameworkCore.r.Next(192, 384);
                    link.linkObject.pos.Y = node1.pos.Y + keyOffset;
                    keyOffset += 96;
                }
            }
        }

        private void fileListing(List<string> fileListing, string directory)
        {
            //root directory.
            GetFilesFromDirectory(fileListing, directory);

            //subDirectories.
            string[] subDirs = Directory.GetDirectories(directory);
            for (int i = 0; i < subDirs.Length; i++)
            {
                GetFilesFromDirectory(fileListing, subDirs[i]);
            }
        }



        private void GetFilesFromDirectory(List<string> fileListing, string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] rgFiles = di.GetFiles("ev*.cs");

            foreach (FileInfo fi in rgFiles)
            {
                if (string.Compare(fi.Name, "eventmanager.cs", true) == 0)
                    continue;

                if (string.Compare(fi.Name, "eventpopup.cs", true) == 0)
                    continue;

              

                //if (string.Compare(fi.Name, "evRuins.cs", true) != 0)
                    //continue;



                fileListing.Add(fi.FullName);
            }
        }

        


        public override void Draw(GameTime gameTime)
        {
            Rectangle fullRect = new Rectangle(
                0, 0,
                (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            Color bg =  Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                fullRect, sprite.blank, bg);



            if (evDebugNodes == null)
                return;

            Vector2 lineVec = FrameworkCore.Serif.MeasureString("Sample");
            lineVec.Y *= fontSize;
            lineVec.X *= fontSize;

            foreach (evDebugNode node in evDebugNodes)
            {
                DrawTextBox(node.pos + evDebugPos, node.fileName, new Color(255,100,0));

                Vector2 funcPos = node.pos;
                funcPos.Y += lineVec.Y;
                foreach (string funcName in node.functionNames)
                {
                    DrawTextBox(funcPos + evDebugPos, funcName, Color.Black);
                    funcPos.Y += lineVec.Y;
                }


                if (node.unlockLinks.Count > 0)
                {
                    foreach (linkString node2 in node.unlockLinks)
                    {
                        if (node2.linkObject == null)
                            continue;

                        Vector2 length = FrameworkCore.Serif.MeasureString(node.functionNames[node2.funcNum ]);
                        length.X *= fontSize;
                        length.Y *= fontSize;
                        Vector2 startPos = node.pos + new Vector2(length.X, (node2.funcNum + 1.5f) * length.Y) + evDebugPos;

                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                            new Rectangle(
                                (int)startPos.X,
                                (int)startPos.Y - 4,
                                32,
                                8),
                                sprite.blank, new Color(160, 160, 160));
                        
                        DrawLine(startPos + new Vector2(32,0),
                            node2.linkObject.pos + evDebugPos, new Color(160,160,160), Color.Black);
                    }
                }

                if (node.keyLinks.Count > 0)
                {
                    foreach (linkString node2 in node.keyLinks)
                    {
                        if (node2.linkObject == null)
                            continue;

                        Vector2 length = FrameworkCore.Serif.MeasureString(node.functionNames[node2.funcNum ]);
                        length.X *= fontSize;
                        length.Y *= fontSize;

                        Vector2 startPos = node.pos + new Vector2(length.X, (node2.funcNum + 1.5f) * length.Y) + evDebugPos;

                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                            new Rectangle(
                                (int)startPos.X,
                                (int)startPos.Y - 4,
                                32,
                                8),
                                sprite.blank, new Color(255, 200, 0));

                        DrawLine(startPos + new Vector2(32, 0),
                            node2.linkObject.pos + evDebugPos, new Color(255, 200, 0), new Color(255,120,0));
                    }
                }
            }
        }

        private void DrawLine(Vector2 p1, Vector2 p2, Color color1, Color color2)
        {
            try
            {
                for (float i = 0; i < 1; i += 0.003f)
                {
                    float lerp = MathHelper.Clamp(i, 0, 1);
                    Color color = Color.Lerp(color1, color2, lerp);
                    Vector2 dotPos = Vector2.Lerp(p1, p2, lerp);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, dotPos, sprite.blank, color, 0,
                        Helpers.SpriteCenter(sprite.blank), 1, SpriteEffects.None, 0);
                }

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, p2, sprite.blank, color2, 0,
                    Helpers.SpriteCenter(sprite.blank), 2, SpriteEffects.None, 0);
            }
            catch
            {
            }
        }

        private void DrawTextBox(Vector2 pos, string text, Color color)
        {
            Vector2 textVec = FrameworkCore.Serif.MeasureString(text);
            textVec.X *= fontSize;
            textVec.Y *= fontSize;

            Rectangle bgRect = new Rectangle(
                (int)pos.X,
                (int)pos.Y,
                (int)textVec.X,
                (int)textVec.Y);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, bgRect, sprite.blank, new Color(224, 224, 224));
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, text, pos, color);
        }

      
    }
}
