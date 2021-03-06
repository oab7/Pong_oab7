﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pong.Screens;
using Pong.Inputs;

namespace Pong.GameElements
{
    /// <summary>
    /// This is a stack of screens used in the game.
    /// Do not Add or Remove screens yourself. Use the "Play"
    /// method to pop a new screen on the stack, and if you want
    /// to remove a screen, so the GameScreen.Disposed value
    /// to true for a screen and it will get removed for you.
    /// </summary>
    public class ScreenContainer : List<GameScreen>
    {
        /// <summary>
        /// Gets a value indicating whether the game is paused.
        /// </summary>
        /// <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// This is the screen that will be added as soon as this instance updates.
        /// </summary>
        private GameScreen toAdd;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenContainer"/> class.
        /// </summary>
        public ScreenContainer()
            : base()
        {
            this.IsPaused = false;
            this.toAdd = null;
        }

        /// <summary>
        /// Plays the specified Screen.
        /// This will not happen until Update is called.
        /// </summary>
        /// <param name="toAdd">To new GameScreen to be added.</param>
        public void Play(GameScreen toAdd)
        {
            this.toAdd = toAdd;
        }

        /// <summary>
        /// Adds the screen in "toAdd" to the stack.
        /// </summary>
        private void Add()
        {
            if (this.toAdd != null)
            {
                this.Add(this.toAdd);
                this.toAdd = null;
            }
        }

        /// <summary>
        /// Finds all screens which have the value of
        /// "Disposed" set to true and removes them.
        /// </summary>
        private void Kill()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Disposed)
                {
                    if ((this[i] as PauseScreen) != null)
                    {
                        this.IsPaused = false;
                    }

                    Remove(this[i]);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes all screens from this stack. Update
        /// must be called after this for this to take effect.
        /// </summary>
        public void KillAll()
        {
            foreach (GameScreen screen in this)
            {
                screen.Disposed = true;
            }
        }

        /// <summary>
        /// Pauses the game and adds a Pause Screen to the stack.
        /// </summary>
        public void Pause()
        {
            this.Add(new PauseScreen());
            this.IsPaused = true;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            this.Kill();
            this.Add();

            // Update the screens from top to bottom, stopping when a
            // screen is found that is not "fading out".
            for (int i = Count - 1; i >= 0; i--)
            {
                this[i].Update();
                if (!this[i].FadingOut)
                {
                    break;
                }
            }

            // Check if the game is being paused, and there is no pause screen on the stack.
            if (!this.IsPaused && GameWorld.controller.ContainsBool(ActionType.Pause))
            {
                this.Pause();
            }
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void Draw()
        {
            foreach (GameScreen screen in this)
            {
                screen.Draw();
            }
        }
    }
}