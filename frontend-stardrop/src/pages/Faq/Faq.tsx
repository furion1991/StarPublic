import Image from 'next/image'

import { AccordionItem } from '@/shared/ui'
import faqData from '@/shared/config/faq.json'

import classes from './Faq.module.scss'

export const FaqPage = () => {
  const replaceLogoSymbol = (text: string) => {
    if (!text.includes('🌟')) {
      return text
    }

    const parts = text.split('🌟')

    return parts.map((part, index) => (
      <p className={classes.paragraphWithLogo} key={index}>
        {part}
        {index < parts.length - 1 && (
          <Image src='/icons/logo-mini.svg' width={19} height={19} alt='лого' />
        )}
      </p>
    ))
  }

  return (
    <div className={classes.faqPage}>
      <div className={classes.wrapper}>
        <h1>Ответы на часто задаваемые вопросы</h1>

        <div className={classes.faqList}>
          {faqData.map(({ question, answer }) => {
            return (
              <AccordionItem key={question} heading={replaceLogoSymbol(question)}>
                <p className={classes.answerText}>{replaceLogoSymbol(answer)}</p>
              </AccordionItem>
            )
          })}
        </div>
      </div>
    </div>
  )
}
