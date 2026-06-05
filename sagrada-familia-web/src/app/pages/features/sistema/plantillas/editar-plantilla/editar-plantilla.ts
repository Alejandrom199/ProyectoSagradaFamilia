import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { EditorComponent } from 'ngx-monaco-editor-v2';
import { Breadcrumb, BreadcrumbItem } from '../../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../../core/services/loading-bar';
import { PlantillasService } from '../../../../../core/services/plantillas';
import { PlantillaResponse, PlantillaUpdate } from '../../../../../shared/interfaces/plantilla.interface';

interface Variable {
  token: string;
  descripcion: string;
  plantillas: string[];
}

@Component({
  selector: 'app-editar-plantilla',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, EditorComponent],
  templateUrl: './editar-plantilla.html',
})
export class EditarPlantilla implements OnInit {
  @Input() id!: string;

  private plantillasService = inject(PlantillasService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  private sanitizer = inject(DomSanitizer);

  plantilla = signal<PlantillaResponse | null>(null);
  guardando = signal(false);
  error = signal('');
  submitted = signal(false);
  vistaPrevia = signal(false);
  private editorInstance: any = null;

  onEditorInit(editor: any): void {
    this.editorInstance = editor;
  }

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

  get htmlSeguro(): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(this.form.cuerpo);
  }

  form = {
    nombre: '',
    asunto: '',
    cuerpo: '',
    activo: true,
  };

  readonly variables: Variable[] = [
    { token: '{{NOMBRE}}',   descripcion: 'Nombre del destinatario',    plantillas: ['CAMBIO_CLAVE', 'CUENTA_PADRE', 'CUENTA_MEDICO'] },
    { token: '{{APELLIDO}}', descripcion: 'Apellido del destinatario',   plantillas: ['CUENTA_PADRE', 'CUENTA_MEDICO'] },
    { token: '{{LINK}}',     descripcion: 'Enlace de acción (botón)',    plantillas: ['CAMBIO_CLAVE', 'CUENTA_PADRE', 'CUENTA_MEDICO'] },
    { token: '{{EMAIL}}',    descripcion: 'Correo del destinatario',     plantillas: ['CUENTA_MEDICO'] },
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Sistema' },
    { label: 'Plantillas de correo', ruta: '/sistema/plantillas' },
    { label: 'Editar' },
  ];

  ngOnInit(): void {
    this.loadingBar.show();
    this.plantillasService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.plantilla.set(r.data);
          this.form = {
            nombre:  r.data.nombre,
            asunto:  r.data.asunto,
            cuerpo:  r.data.cuerpo,
            activo:  r.data.activo,
          };
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  variablesAplicables(): Variable[] {
    const codigo = this.plantilla()?.codigo ?? '';
    return this.variables.filter(v => v.plantillas.includes(codigo));
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

    const request: PlantillaUpdate = {
      nombre:  this.form.nombre.trim(),
      asunto:  this.form.asunto.trim(),
      cuerpo:  this.form.cuerpo,
      activo:  this.form.activo,
    };

    this.plantillasService.actualizar(parseInt(this.id), request).subscribe({
      next: (r) => {
        if (r.success) this.router.navigate(['/sistema/plantillas']);
        else this.error.set(r.message);
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al guardar.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
