// MainMenuForm.Designer.cs
namespace MyGame
{
    partial class MainMenuForm
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
            // Este método está vacío porque estás creando los controles manualmente
            // en el archivo .cs principal (CreateMenuButtons).
            // Si usas el diseñador de VS, aquí se generará el código automáticamente.
            this.components = new System.ComponentModel.Container();
            this.Text = "Knightmare - Main Menu";
            this.ClientSize = new System.Drawing.Size(960, 640);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }

        #endregion
    }
}