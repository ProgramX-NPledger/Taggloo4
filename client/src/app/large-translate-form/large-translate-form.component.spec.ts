import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LargeTranslateFormComponent } from './large-translate-form.component';

describe('LargeTranslateFormComponent', () => {
  let component: LargeTranslateFormComponent;
  let fixture: ComponentFixture<LargeTranslateFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LargeTranslateFormComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LargeTranslateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
