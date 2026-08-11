/**
 * Registro centralizado de íconos de la aplicación.
 * - APP_ICONS se registra globalmente en app.config.ts → ningún componente necesita su propio provideIcons
 * - SIDEBAR_ICON_MAP traduce las claves del backend a nombres de ícono ng-icons
 */

// ── Heroicons (UI components: datatable, modales, formularios, etc.) ──────────
import {
  heroHome, heroChevronRight, heroChevronLeft, heroChevronDown,
  heroChevronDoubleLeft, heroChevronDoubleRight,
  heroAdjustmentsHorizontal,
  heroArrowLeft, heroArrowRight, heroArrowUp, heroArrowDown,
  heroArrowPath, heroArrowRightOnRectangle, heroArrowTrendingUp,
  heroBars3, heroBeaker, heroBriefcase,
  heroCake, heroCalendarDays, heroChartBar, heroChartBarSquare,
  heroCheckCircle, heroCircleStack, heroClock, heroCloud,
  heroCog6Tooth, heroCog8Tooth,
  heroDocumentArrowDown, heroDocumentText,
  heroEllipsisVertical, heroEnvelope, heroEye, heroEyeSlash,
  heroExclamationCircle, heroExclamationTriangle,
  heroFaceFrown, heroFaceSmile, heroFunnel,
  heroHeart, heroIdentification, heroKey,
  heroLockClosed, heroLockOpen,
  heroMagnifyingGlass, heroMoon,
  heroPencil, heroPencilSquare, heroPhone, heroPlus,
  heroScale, heroShieldCheck, heroSparkles, heroSun,
  heroTableCells, heroTag, heroTrash,
  heroUser, heroUserCircle, heroUserGroup, heroUserPlus, heroUsers,
  heroViewColumns,
  heroXCircle, heroXMark,
  heroAcademicCap, heroArchiveBox, heroCheck, heroClipboardDocumentList
} from '@ng-icons/heroicons/outline';

// ── Material Icons Outline (sidebar / menú principal) ────────────────────────
import {
  matHomeOutline,
  matPeopleOutline,
  matGroupsOutline,
  matBadgeOutline,
  matAdminPanelSettingsOutline,
  matTuneOutline,
  matCalendarMonthOutline,
  matScheduleOutline,
  matSearchOutline,
  matWarningAmberOutline,
  matTrendingUpOutline,
  matBatchPredictionOutline,
  matSetMealOutline,
  matTableChartOutline,
  matBarChartOutline,
  matMonitorHeartOutline,
  matAssignmentOutline,
  matSettingsOutline,
  matManageAccountsOutline,
  matChildCareOutline,
  matMedicalServicesOutline,
  matInventory2Outline,
  matVerifiedUserOutline,
  matLocalHospitalOutline,
  matEventNoteOutline,
  matGridViewOutline,
  // Alimentos / importación
  matCategoryOutline,
  matEditNoteOutline,
  matFileUploadOutline,
  matCloudUploadOutline,
  matInsertDriveFileOutline,
  matTaskAltOutline,
  matCheckCircleOutline,
  matCloseOutline,
  matErrorOutline,
  matReportProblemOutline,
  matNoMealsOutline,
  // Personas / usuarios
  matPersonOutline,
  matPersonAddOutline,
  matAccountCircleOutline,
  // Contacto / autenticación
  matEmailOutline,
  matMailOutline,
  matPhoneOutline,
  matVpnKeyOutline,
  matLockOutline,
  matLockOpenOutline,
  // Médico / clínico
  matSchoolOutline,
  matWorkOutline,
  matScienceOutline,
  matCakeOutline,
  matMonitorWeightOutline,
  matDescriptionOutline,
  // Tiempo
  matAccessTimeOutline,
  // Estados / UI
  matCancelOutline,
  matSentimentSatisfiedOutline,
  matSentimentDissatisfiedOutline,
  // Dashboard / navegación
  matArrowForwardOutline,
  matFavoriteBorderOutline,
  matQueryStatsOutline,
  // Explorador visual
  matAutoAwesomeOutline,
  // Acciones UI globales
  matAddOutline,
  matVisibilityOutline, matVisibilityOffOutline,
  matCheckOutline,
  matDeleteOutline,
  matChevronLeftOutline, matChevronRightOutline, matExpandMoreOutline,
  matLogoutOutline,
  matMenuOutline,
  matEditOutline,
  // Categorías orientación padres
  matPublicOutline,
  matEcoOutline,
  // Medidas progreso
  matHeightOutline,
  matHistoryOutline,
  matStraightenOutline,
  // Dashboard admin
  matHourglassBottomOutline,
  matBoltOutline,
  matInfoOutline,
  // Crear usuario
  matGroupOutline,
  // Reagendación de citas
  matEditCalendarOutline
} from '@ng-icons/material-icons/outline';

// ── Font Awesome Solid (íconos de dominio sin equivalente en Hero/Material NI en
//    el set "regular" de Font Awesome — ver bloque "Regular" más abajo) ──────────
import {
  // Pacientes / niños
  faSolidBaby, faSolidChild, faSolidBabyCarriage, faSolidHandsHoldingChild,
  // Médicos / clínico
  faSolidUserDoctor, faSolidUserNurse, faSolidStethoscope, faSolidSyringe,
  faSolidNotesMedical, faSolidFileMedical, faSolidHeartPulse,
  faSolidFilePrescription,
  // Medidas / crecimiento
  faSolidWeightScale, faSolidScaleBalanced,
  // Nutrición / alimentos
  faSolidUtensils, faSolidCarrot, faSolidAppleWhole,
  // Predicción / IA
  faSolidBrain, faSolidRobot, faSolidMagnifyingGlassChart,
  // Citas
  faSolidEnvelopeCircleCheck,
  // Reportes
  faSolidChartLine, faSolidChartPie
} from '@ng-icons/font-awesome/solid';

// ── Font Awesome Regular (variante outline; se usa cuando Font Awesome Free trae
//    un equivalente "regular" del ícono — si no existe, se usa el "solid" de arriba) ──
import {
  // Médicos / clínico
  faHospital as faRegularHospital,
  // Citas
  faCalendarCheck as faRegularCalendarCheck,
  // Datatable — Excel / PDF / exportar / importar
  faFileExcel as faRegularFileExcel,
  faFilePdf as faRegularFilePdf,
  faShareFromSquare as faRegularShareFromSquare,
  faFolderOpen as faRegularFolderOpen,
  faFileLines as faRegularFileLines,
} from '@ng-icons/font-awesome/regular';

// ── Exportación combinada para provideIcons global ────────────────────────────
export const APP_ICONS = {
  // Heroicons
  heroHome, heroChevronRight, heroChevronLeft, heroChevronDown,
  heroChevronDoubleLeft, heroChevronDoubleRight,
  heroAdjustmentsHorizontal,
  heroArrowLeft, heroArrowRight, heroArrowUp, heroArrowDown,
  heroArrowPath, heroArrowRightOnRectangle, heroArrowTrendingUp,
  heroBars3, heroBeaker, heroBriefcase,
  heroCake, heroCalendarDays, heroChartBar, heroChartBarSquare,
  heroCheckCircle, heroCircleStack, heroClock, heroCloud,
  heroCog6Tooth, heroCog8Tooth,
  heroDocumentArrowDown, heroDocumentText,
  heroEllipsisVertical, heroEnvelope, heroEye, heroEyeSlash,
  heroExclamationCircle, heroExclamationTriangle,
  heroFaceFrown, heroFaceSmile, heroFunnel,
  heroHeart, heroIdentification, heroKey,
  heroLockClosed, heroLockOpen,
  heroMagnifyingGlass, heroMoon,
  heroPencil, heroPencilSquare, heroPhone, heroPlus,
  heroScale, heroShieldCheck, heroSparkles, heroSun,
  heroTableCells, heroTag, heroTrash,
  heroUser, heroUserCircle, heroUserGroup, heroUserPlus, heroUsers,
  heroViewColumns,
  heroXCircle, heroXMark,
  heroAcademicCap, heroArchiveBox, heroCheck, heroClipboardDocumentList,
  // Material Icons
  matHomeOutline,
  matPeopleOutline,
  matGroupsOutline,
  matBadgeOutline,
  matAdminPanelSettingsOutline,
  matTuneOutline,
  matCalendarMonthOutline,
  matScheduleOutline,
  matSearchOutline,
  matWarningAmberOutline,
  matTrendingUpOutline,
  matBatchPredictionOutline,
  matSetMealOutline,
  matTableChartOutline,
  matBarChartOutline,
  matMonitorHeartOutline,
  matAssignmentOutline,
  matSettingsOutline,
  matManageAccountsOutline,
  matChildCareOutline,
  matMedicalServicesOutline,
  matInventory2Outline,
  matVerifiedUserOutline,
  matLocalHospitalOutline,
  matEventNoteOutline,
  matGridViewOutline,
  // Alimentos / importación
  matCategoryOutline,
  matEditNoteOutline,
  matFileUploadOutline,
  matCloudUploadOutline,
  matInsertDriveFileOutline,
  matTaskAltOutline,
  matCheckCircleOutline,
  matCloseOutline,
  matErrorOutline,
  matReportProblemOutline,
  matNoMealsOutline,
  // Personas / usuarios
  matPersonOutline,
  matPersonAddOutline,
  matAccountCircleOutline,
  // Contacto / autenticación
  matEmailOutline,
  matMailOutline,
  matPhoneOutline,
  matVpnKeyOutline,
  matLockOutline,
  matLockOpenOutline,
  // Médico / clínico
  matSchoolOutline,
  matWorkOutline,
  matScienceOutline,
  matCakeOutline,
  matMonitorWeightOutline,
  matDescriptionOutline,
  // Tiempo
  matAccessTimeOutline,
  // Estados / UI
  matCancelOutline,
  matSentimentSatisfiedOutline,
  matSentimentDissatisfiedOutline,
  // Dashboard / navegación
  matArrowForwardOutline,
  matFavoriteBorderOutline,
  matQueryStatsOutline,
  // Explorador visual
  matAutoAwesomeOutline,
  // Acciones UI globales
  matAddOutline,
  matVisibilityOutline, matVisibilityOffOutline,
  matCheckOutline,
  matDeleteOutline,
  matChevronLeftOutline, matChevronRightOutline, matExpandMoreOutline,
  matLogoutOutline,
  matMenuOutline,
  matEditOutline,
  // Categorías orientación padres
  matPublicOutline,
  matEcoOutline,
  // Medidas progreso
  matHeightOutline,
  matHistoryOutline,
  matStraightenOutline,
  // Dashboard admin
  matHourglassBottomOutline,
  matBoltOutline,
  matInfoOutline,
  // Crear usuario
  matGroupOutline,
  // Reagendación de citas
  matEditCalendarOutline,
  // Font Awesome Solid (sin equivalente "regular" en Font Awesome Free)
  // Pacientes / niños
  faSolidBaby, faSolidChild, faSolidBabyCarriage, faSolidHandsHoldingChild,
  // Médicos / clínico
  faSolidUserDoctor, faSolidUserNurse, faSolidStethoscope, faSolidSyringe,
  faSolidNotesMedical, faSolidFileMedical, faSolidHeartPulse,
  faSolidFilePrescription,
  // Medidas / crecimiento
  faSolidWeightScale, faSolidScaleBalanced,
  // Nutrición / alimentos
  faSolidUtensils, faSolidCarrot, faSolidAppleWhole,
  // Predicción / IA
  faSolidBrain, faSolidRobot, faSolidMagnifyingGlassChart,
  // Citas
  faSolidEnvelopeCircleCheck,
  // Reportes
  faSolidChartLine, faSolidChartPie,
  // Font Awesome Regular (variante outline, cuando existe equivalente)
  faRegularHospital, faRegularCalendarCheck,
  // Datatable — Excel / PDF / exportar / importar
  faRegularFileExcel, faRegularFilePdf,
  faRegularShareFromSquare, faRegularFolderOpen, faRegularFileLines,
};

/**
 * Mapeo backend-key → nombre ng-icon para el sidebar.
 * Las claves las envía el backend (MenuResponse.icono / ModuloMenu.icono).
 */
export const SIDEBAR_ICON_MAP: Record<string, string> = {
  // Módulos
  'settings': 'matSettingsOutline',
  'monitor-heart': 'matMonitorHeartOutline',
  'assignment': 'matAssignmentOutline',
  'batch-prediction': 'matBatchPredictionOutline',
  'set-meal': 'matSetMealOutline',
  'table-chart': 'matTableChartOutline',
  // Opciones de gestión
  'people': 'matPeopleOutline',
  'groups': 'matGroupsOutline',
  'badge': 'matBadgeOutline',
  'admin-panel': 'matAdminPanelSettingsOutline',
  'manage-accounts': 'matManageAccountsOutline',
  'verified-user': 'matVerifiedUserOutline',
  'user-doctor': 'faSolidUserDoctor',
  'tune': 'matTuneOutline',
  'category': 'matCategoryOutline',
  // Opciones clínicas
  'calendar-month': 'matCalendarMonthOutline',
  'calendar-check': 'faRegularCalendarCheck',
  'schedule': 'matScheduleOutline',
  'history': 'matHistoryOutline',
  'search': 'matSearchOutline',
  'event-note': 'matEventNoteOutline',
  'child-care': 'matChildCareOutline',
  'medical-services': 'matMedicalServicesOutline',
  'local-hospital': 'matLocalHospitalOutline',
  'notes-medical': 'faSolidNotesMedical',
  'file-prescription': 'faSolidFilePrescription',
  // Reportes / predicción
  'trending-up': 'matTrendingUpOutline',
  'bar-chart': 'matBarChartOutline',
  'grid-view': 'matGridViewOutline',
  'brain': 'faSolidBrain',
  'chart-line': 'faSolidChartLine',
  // Nutrición
  'utensils': 'faSolidUtensils',
  // Auditoría / alertas
  'warning-amber': 'matWarningAmberOutline',
  // Comunicación
  'email': 'matEmailOutline',
  'envelope-check': 'faSolidEnvelopeCircleCheck',
  // Utilidades
  'home': 'matHomeOutline',
  // ── Claves legadas del backend (nombres heroicons) — retrocompatibilidad ──
  'cog-6-tooth': 'matSettingsOutline',
  'heart': 'matMonitorHeartOutline',
  'clipboard-document-list': 'matAssignmentOutline',
  'arrow-trending-up': 'matTrendingUpOutline',
  'archive-box': 'matSetMealOutline',
  'table-cells': 'matTableChartOutline',
  'users': 'matPeopleOutline',
  'user-group': 'matGroupsOutline',
  'identification': 'matBadgeOutline',
  'shield-check': 'matAdminPanelSettingsOutline',
  'calendar-days': 'matCalendarMonthOutline',
  'clock': 'matScheduleOutline',
  'magnifying-glass': 'matSearchOutline',
  'exclamation-triangle': 'matWarningAmberOutline',
  'chart-bar': 'matBarChartOutline',
};
