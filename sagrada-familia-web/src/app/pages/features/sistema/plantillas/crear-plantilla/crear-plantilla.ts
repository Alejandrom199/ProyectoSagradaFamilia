import { Component, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { EditorComponent } from 'ngx-monaco-editor-v2';
import { Breadcrumb, BreadcrumbItem } from '../../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../../core/services/loading-bar';
import { PlantillasService } from '../../../../../core/services/plantillas';
import { PlantillaCreate } from '../../../../../shared/interfaces/plantilla.interface';

interface Variable {
  token:       string;
  descripcion: string;
}

@Component({
  selector: 'app-crear-plantilla',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, EditorComponent],
  templateUrl: './crear-plantilla.html',
})
export class CrearPlantilla {
  private readonly plantillasService = inject(PlantillasService);
  private readonly router            = inject(Router);
  private readonly loadingBar        = inject(LoadingBar);
  private readonly sanitizer         = inject(DomSanitizer);

  guardando  = signal(false);
  error      = signal('');
  submitted  = signal(false);
  vistaPrevia = signal(false);
  private editorInstance: any = null;

  readonly editorOptions = {
    language: 'html',
    theme: 'vs',
    automaticLayout: true,
    minimap: { enabled: false },
    wordWrap: 'on' as const,
    fontSize: 13,
    lineNumbers: 'on' as const,
    scrollBeyondLastLine: false,
    padding: { top: 12, bottom: 12 },
    tabSize: 2,
  };

  form = { nombre: '', asunto: '', cuerpo: '' };

  // Todas las variables disponibles en el sistema
  readonly variables: Variable[] = [
    { token: '{{NOMBRE}}',   descripcion: 'Nombre del destinatario' },
    { token: '{{APELLIDO}}', descripcion: 'Apellido del destinatario' },
    { token: '{{EMAIL}}',    descripcion: 'Correo electrónico del destinatario' },
    { token: '{{LINK}}',     descripcion: 'Enlace de acción principal (activación / reset)' },
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Sistema' },
    { label: 'Plantillas de correo', ruta: '/sistema/plantillas' },
    { label: 'Nueva plantilla' },
  ];

  get htmlSeguro(): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(this.form.cuerpo);
  }

  onEditorInit(editor: any): void {
    this.editorInstance = editor;
  }

  insertar(token: string): void {
    if (!this.editorInstance) return;
    this.editorInstance.executeEdits('insert-variable', [{
      range: this.editorInstance.getSelection(),
      text: token,
      forceMoveMarkers: true,
    }]);
    this.editorInstance.focus();
  }

  guardar(): void {
    this.submitted.set(true);
    this.error.set('');

    if (!this.form.nombre.trim() || !this.form.asunto.trim() || !this.form.cuerpo.trim()) {
      this.error.set('Completá los campos obligatorios marcados con *.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    const request: PlantillaCreate = {
      nombre: this.form.nombre.trim(),
      asunto: this.form.asunto.trim(),
      cuerpo: this.form.cuerpo,
    };

    this.plantillasService.crear(request).subscribe({
      next: (r) => {
        if (r.success) this.router.navigate(['/sistema/plantillas']);
        else this.error.set(r.message);
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al crear la plantilla.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
