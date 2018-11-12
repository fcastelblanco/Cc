namespace Cc.Upt.Notificator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.textBoxNotification = new System.Windows.Forms.TextBox();
            this.buttonMinimize = new System.Windows.Forms.Button();
            this.labelShowNotificationAfter = new System.Windows.Forms.Label();
            this.textBoxEventsQuantity = new System.Windows.Forms.TextBox();
            this.labelEvents = new System.Windows.Forms.Label();
            this.labelNotificationExpirationTime = new System.Windows.Forms.Label();
            this.textBoxNotificationExpiration = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Visible = true;
            this.notifyIcon.BalloonTipClicked += new System.EventHandler(this.notifyIcon_BalloonTipClicked);
            this.notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
            // 
            // textBoxNotification
            // 
            this.textBoxNotification.BackColor = System.Drawing.Color.White;
            this.textBoxNotification.Location = new System.Drawing.Point(13, 13);
            this.textBoxNotification.Multiline = true;
            this.textBoxNotification.Name = "textBoxNotification";
            this.textBoxNotification.ReadOnly = true;
            this.textBoxNotification.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxNotification.Size = new System.Drawing.Size(705, 215);
            this.textBoxNotification.TabIndex = 0;
            // 
            // buttonMinimize
            // 
            this.buttonMinimize.Location = new System.Drawing.Point(13, 235);
            this.buttonMinimize.Name = "buttonMinimize";
            this.buttonMinimize.Size = new System.Drawing.Size(75, 23);
            this.buttonMinimize.TabIndex = 1;
            this.buttonMinimize.Text = "Minimizar";
            this.buttonMinimize.UseVisualStyleBackColor = true;
            this.buttonMinimize.Click += new System.EventHandler(this.buttonMinimize_Click);
            // 
            // labelShowNotificationAfter
            // 
            this.labelShowNotificationAfter.AutoSize = true;
            this.labelShowNotificationAfter.Location = new System.Drawing.Point(12, 261);
            this.labelShowNotificationAfter.Name = "labelShowNotificationAfter";
            this.labelShowNotificationAfter.Size = new System.Drawing.Size(157, 13);
            this.labelShowNotificationAfter.TabIndex = 2;
            this.labelShowNotificationAfter.Text = "Mostrar notificación despues de";
            // 
            // textBoxEventsQuantity
            // 
            this.textBoxEventsQuantity.Location = new System.Drawing.Point(175, 258);
            this.textBoxEventsQuantity.Name = "textBoxEventsQuantity";
            this.textBoxEventsQuantity.Size = new System.Drawing.Size(50, 20);
            this.textBoxEventsQuantity.TabIndex = 3;
            this.textBoxEventsQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxEventsQuantity_KeyPress);
            // 
            // labelEvents
            // 
            this.labelEvents.AutoSize = true;
            this.labelEvents.Location = new System.Drawing.Point(231, 261);
            this.labelEvents.Name = "labelEvents";
            this.labelEvents.Size = new System.Drawing.Size(45, 13);
            this.labelEvents.TabIndex = 4;
            this.labelEvents.Text = "eventos";
            // 
            // labelNotificationExpirationTime
            // 
            this.labelNotificationExpirationTime.AutoSize = true;
            this.labelNotificationExpirationTime.Location = new System.Drawing.Point(12, 285);
            this.labelNotificationExpirationTime.Name = "labelNotificationExpirationTime";
            this.labelNotificationExpirationTime.Size = new System.Drawing.Size(203, 13);
            this.labelNotificationExpirationTime.TabIndex = 5;
            this.labelNotificationExpirationTime.Text = "Duración de la notificación (en segundos)";
            // 
            // textBoxNotificationExpiration
            // 
            this.textBoxNotificationExpiration.Location = new System.Drawing.Point(221, 282);
            this.textBoxNotificationExpiration.Name = "textBoxNotificationExpiration";
            this.textBoxNotificationExpiration.Size = new System.Drawing.Size(50, 20);
            this.textBoxNotificationExpiration.TabIndex = 6;
            this.textBoxNotificationExpiration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNotificationExpiration_KeyPress);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(730, 317);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxNotificationExpiration);
            this.Controls.Add(this.labelNotificationExpirationTime);
            this.Controls.Add(this.labelEvents);
            this.Controls.Add(this.textBoxEventsQuantity);
            this.Controls.Add(this.labelShowNotificationAfter);
            this.Controls.Add(this.buttonMinimize);
            this.Controls.Add(this.textBoxNotification);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ipm notificator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.TextBox textBoxNotification;
        private System.Windows.Forms.Button buttonMinimize;
        private System.Windows.Forms.Label labelShowNotificationAfter;
        private System.Windows.Forms.TextBox textBoxEventsQuantity;
        private System.Windows.Forms.Label labelEvents;
        private System.Windows.Forms.Label labelNotificationExpirationTime;
        private System.Windows.Forms.TextBox textBoxNotificationExpiration;
    }
}

