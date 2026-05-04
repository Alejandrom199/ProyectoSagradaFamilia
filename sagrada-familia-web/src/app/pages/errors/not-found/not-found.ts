import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroArrowLeft, heroFaceFrown } from '@ng-icons/heroicons/outline';

@Component({
  selector: 'app-not-found',
  imports: [NgIcon],
  viewProviders: [provideIcons({ heroFaceFrown, heroArrowLeft })],
  templateUrl: './not-found.html',
  styleUrl: './not-found.css',
})
export class NotFound {

}
