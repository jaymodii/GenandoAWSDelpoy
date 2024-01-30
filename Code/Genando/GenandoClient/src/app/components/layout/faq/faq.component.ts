import { Component, Input } from '@angular/core';
import { Faq } from 'src/app/models/faq';
import { IResponse } from 'src/app/models/shared/response';
import { FaqService } from 'src/app/services/faq.service';
import { Panel } from 'src/app/shared/models/panel';

@Component({
  selector: 'app-faq',
  templateUrl: './faq.component.html',
  styleUrls: ['./faq.component.scss'],
})
export class FaqComponent {
  panels: Panel[] = [];
  faq!: Faq[];

  constructor(private faqService: FaqService) {}

  ngOnInit(): void {
    this.faqService.getFaq().subscribe((response: IResponse<Faq[]>) => {
      this.faq = response.data;
      for (let item of this.faq) {
        const panel = {
          name: item.question,
          content: item.answer,
          duration: 0,
        } as Panel;
        this.panels.push(panel);
      }
    });
  }
}
