import Image from 'next/image'
import { Fragment } from 'react'

import userAgreementData from '@/shared/config/user-agreement.json'

import classes from './UserAgreement.module.scss'

type SubSection = {
  text: string
  items?: SubSection[]
  description?: {
    text: string
  }[]
}

export const UserAgreementPage = () => {
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
    <div className={classes.userAgreementPage}>
      <div className={classes.wrapper}>
        <h1>
          <Image src='/icons/logo.svg' width={55} height={54} alt='лого' /> StarDrop
        </h1>

        <div className={classes.warning}>
          <Image
            src='/icons/exclamation-mark.svg'
            width={56}
            height={56}
            alt='Восклицательный знак'
          />

          <div className={classes.warningRight}>
            <p>Внимание!</p>

            <p>
              Пожалуйста, ознакомьтесь с настоящим пользовательским соглашением до начала
              использования сайта stardrop.app и его программных средств.
            </p>

            <p>
              Регистрация (авторизация) на сайте будет означать ваше согласие с условиями настоящего
              пользовательского соглашения.
            </p>

            <p>
              Если Вы не согласны с условиями настоящего пользовательского соглашения, не
              регистрируйтесь (авторизируйтесь) на сайте и не используйте его программные средства.
            </p>
          </div>
        </div>

        <h2>Пользовательское соглашение</h2>

        <p className={classes.updateDate}>Обновлено от 02.06.2025</p>

        <div className={classes.sections}>
          {userAgreementData.map(({ title, description, items }, sectionIdx) => {
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

        <div className={classes.refundPolicy}>
          <h2>Политика возвратов</h2>

          <p>
            Наши Продукты представляют собой цифровой контент (виртуальные предметы), не являются
            товарами в понимании «О защите прав потребителей» и в силу своей специфики не подлежат
            общему праву потребителя на возврат.
          </p>

          <p>
            Возврат денежных средств за приобретенный контент на Сайте возможен по усмотрению
            Администрации. Возврат невозможен, если средства были потрачены или преумножены, либо
            если прошло более 30 календарных дней с момента пополнения счёта.
          </p>

          <p>Возврат осуществляется по тем же реквизитам, с которых была совершена оплата.</p>
        </div>
      </div>
    </div>
  )
}
