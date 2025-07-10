import { Fragment } from 'react'

import privacyPolicyData from '@/shared/config/privacy-policy.json'

import classes from './PrivacyPolicy.module.scss'

type SubSection = {
  text: string
  items?: SubSection[]
  description?: {
    text: string
  }[]
}

export const PrivacyPolicyPage = () => {
  const renderSubSections = (subSections: SubSection[], ...sectionNumbers: number[]) => {
    return subSections.map(({ text, description, items }, subSecitonIdx) => {
      const subSectionNumber = subSecitonIdx + 1

      const previousSectionsNumbers = sectionNumbers.map((sectionNumber, idx, arr) => (
        <Fragment key={idx}>
          {sectionNumber}
          {arr.length - 1 !== idx ? '.' : null}
        </Fragment>
      ))

      if (items) {
        return (
          <Fragment key={text}>
            <li>
              <p>
                <span className={classes.sectionNumber}>
                  {previousSectionsNumbers}.{subSectionNumber}.
                </span>{' '}
                {text}
              </p>

              {description ? description.map(({ text }) => <p key={text}>{text}</p>) : null}
            </li>

            <ul>{renderSubSections(items, ...sectionNumbers, subSectionNumber)}</ul>
          </Fragment>
        )
      }

      return (
        <li key={text}>
          <p>
            <span className={classes.sectionNumber}>
              {previousSectionsNumbers}.{subSectionNumber}.
            </span>{' '}
            {text}
          </p>

          {description ? description.map(({ text }) => <p key={text}>{text}</p>) : null}
        </li>
      )
    })
  }

  return (
    <div className={classes.privacyPolicyPage}>
      <div className={classes.wrapper}>
        <h1>Соглашение о приватности</h1>

        <p className={classes.updateDate}>Обновлено от 02.06.2025</p>

        <div className={classes.sections}>
          {privacyPolicyData.map(({ title, description, items }, sectionIdx) => {
            const sectionNumber = sectionIdx + 1

            return (
              <div key={title}>
                <h3 className={classes.sectionTitle}>
                  <span className={classes.sectionNumber}>{sectionNumber}.</span> {title}
                </h3>

                <p className={classes.sectionDescription}>{description}</p>

                <ul>{renderSubSections(items, sectionNumber)}</ul>
              </div>
            )
          })}
        </div>

        <div className={classes.securityPolicy}>
          <h2>Политика безопасности при оплате банковской картой</h2>

          <p>
            При оплате банковской картой обработка платежа (включая ввод номера карты) происходит на
            защищенной странице процессинговой системы, которая прошла международную сертификацию.
            Это значит, что конфиденциальные данные (реквизиты карты, регистрационные данные и др.)
            полностью защищены и никто, в том числе наш ресурс, не может получить персональные и
            банковские данные клиента. При работе с карточными данными применяется стандарт защиты
            информации, разработанный международными платёжными системами Visa и MasterCard —
            Payment Card Industry Data Security Standard (PCI DSS), что обеспечивает безопасную
            обработку реквизитов Банковской карты Держателя. Применяемая технология передачи данных
            гарантирует безопасность по сделкам с банковскими картами путем использования протоколов
            Secure Sockets Layer (SSL), Verified by Visa, Secure Code и закрытых банковских сетей,
            имеющих высшую степень защиты.
          </p>
        </div>
      </div>
    </div>
  )
}
